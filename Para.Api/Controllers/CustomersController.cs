using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Para.Data.Context;
using Para.Core.Domain;
using Para.Data.UnitOfWork;
using Para.Business.Dtos;
using AutoMapper;

namespace Para.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger <CustomersController> _logger;
        private readonly IMapper _mapper;

        public CustomersController(IUnitOfWork unitOfWork, ILogger<CustomersController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // M��teri ile ili�kili adres , telefon ve detay bilgileriyle birlikte getiriyoruz.
                var customersWithDetails = await _unitOfWork.CustomerRepository.GetWithInclude(
                    c => c.CustomerAddresses,
                    c => c.CustomerPhones,
                    c => c.CustomerDetail
                );


                if (customersWithDetails == null || !customersWithDetails.Any())
                    return NotFound(new { Message = "M��teri bilgisi bulunamad�" });

                return Ok(customersWithDetails);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yap�labilir..
                 _logger.LogError(ex, "M��teri bilgileri getirilirken bir hata olu�tu");
                return StatusCode(500, new { Message = "M��teri bilgileri getirilirken bir hata olu�tu" });
            }
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetByCustomerId(long customerId)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetWithWhereAndInclude(
                    c => c.Id == customerId,
                    c => c.CustomerAddresses,
                    c => c.CustomerPhones,
                    c => c.CustomerDetail
                );

                // Burada yap�lan i�lem veritaban�ndaki sorgu kar��l��� a�a��daki " �rnek " gibidir
                #region �rnek
                    //dbContext.Customers
                    //    .Include(c => c.CustomerAddresses)
                    //    .Include(c => c.CustomerPhones)
                    //    .Include(c => c.CustomerDetail)
                    //    .ToListAsync();
                #endregion

                if (customer == null || !customer.Any())
                {
                    _logger.LogWarning("Belirtilen ID'ye sahip m��teri bulunamad�: {CustomerId}", customerId);
                    return NotFound(new { Message = $"ID: {customerId} olan m��teri bulunamad�" });
                }

                _logger.LogInformation("M��teri ba�ar�yla getirildi. ID: {CustomerId}", customerId);
                return Ok(customer.First());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "M��teri bilgisi getirilirken bir hata olu�tu. ID: {CustomerId}", customerId);
                return StatusCode(500, new { Message = "M��teri bilgisi getirilirken bir hata olu�tu" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchCustomers(string propertyName, string comparison, string value)
        {
            try
            {
                // Dinamik sorgu ile m��terileri getiriyoruz.
                var customers = await _unitOfWork.CustomerRepository.GetWithDynamicQuery(
                    propertyName,
                    comparison,
                    value,
                    c => c.CustomerAddresses,
                    c => c.CustomerPhones
                );

                if (!customers.Any())
                {
                    _logger.LogWarning("M��teri bulunamad�: {PropertyName} {Comparison} {Value}", propertyName, comparison, value);
                    return NotFound(new { Message = "Arama kriterlerine uygun m��teri bulunamad�" });
                }

                _logger.LogInformation("M��teriler getirildi. Say�: {Count}", customers.Count);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "M��teri aramada hata");
                return StatusCode(500, new { Message = "M��teri arama s�ras�nda bir hata olu�tu" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchCustomer([FromBody]CustomerSearchParams searchParams)
        {
            try
            {
                var customerQuery = await _unitOfWork.CustomerRepository.Where(c => true); // Ba�lang��ta t�m m��teriler

                if (!string.IsNullOrWhiteSpace(searchParams.FirstName))
                {
                    customerQuery = customerQuery.Where(c => c.FirstName.Contains(searchParams.FirstName));
                }
                if (!string.IsNullOrWhiteSpace(searchParams.LastName))
                {
                    customerQuery = customerQuery.Where(c => c.LastName == searchParams.LastName);
                }
                if (!string.IsNullOrWhiteSpace(searchParams.Email))
                {
                    customerQuery = customerQuery.Where(c => c.Email.Contains(searchParams.Email));
                }

                if (searchParams.IsActive)
                {
                    customerQuery = customerQuery.Where(c => c.IsActive == searchParams.IsActive);
                }
                // Di�er arama kriterleri buraya eklenebilir ve daha dinamik hale getirilebilir

                var customers = await customerQuery
                    .Include(c => c.CustomerAddresses)
                    .Include(c => c.CustomerPhones)
                    .Include(c => c.CustomerDetail)
                    .ToListAsync();

                if (!customers.Any())
                {
                    _logger.LogInformation("Arama kriterlerine uygun m��teri bulunamad�.");
                    return NotFound(new { Message = "Arama kriterlerine uygun m��teri bulunamad�." });
                }

                _logger.LogInformation("M��teriler ba�ar�yla arand� ve getirildi. Bulunan m��teri say�s�: {Count}", customers.Count);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "M��teri arama i�lemi s�ras�nda bir hata olu�tu.");
                return StatusCode(500, new { Message = "M��teri arama i�lemi s�ras�nda bir hata olu�tu" });
            }
        }

       [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerDto customerDto)
        {
            try
            {
                // Dto olarak veriyi al�yoruz ��nk� ili�kili tablolar var hataya d��memek i�in.
                // Automapper ile mapleyerek veritaban�na g�nderiryoruz.
                var customer = _mapper.Map<Customer>(customerDto);
                await _unitOfWork.CustomerRepository.Insert(customer);
                await _unitOfWork.Complete();

                _logger.LogInformation("Yeni m��teri ba�ar�yla eklendi. ID: {CustomerId}", customer.Id);
                return Ok(new { Message = "M��teri ba�ar�yla eklendi", CustomerId = customer.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "M��teri eklenirken bir hata olu�tu");
                return StatusCode(500, new { Message = "M��teri eklenirken bir hata olu�tu" });
            }
        }

        [HttpPut("{customerId}")]
        public async Task<IActionResult> Put(long customerId, [FromBody] CustomerDto customerDto)
        {
            try
            {
                if (customerId != customerDto.Id)
                {
                    return BadRequest(new { Message = "URL'deki ID ile g�nderilen m��teri ID'si uyu�muyor" });
                }
                var customer = _mapper.Map<Customer>(customerDto);
                await _unitOfWork.CustomerRepository.Update(customer);
                await _unitOfWork.Complete();

                _logger.LogInformation("M��teri ba�ar�yla g�ncellendi. ID: {CustomerId}", customerId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "M��teri g�ncellenirken bir hata olu�tu. ID: {CustomerId}", customerId);
                return StatusCode(500, new { Message = "M��teri g�ncellenirken bir hata olu�tu" });
            }
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> Delete(long customerId)
        {
            try
            {
                await _unitOfWork.CustomerRepository.Delete(customerId);
                await _unitOfWork.Complete();

                _logger.LogInformation("M��teri ba�ar�yla silindi. ID: {CustomerId}", customerId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "M��teri silinirken bir hata olu�tu. ID: {CustomerId}", customerId);
                return StatusCode(500, new { Message = "M��teri silinirken bir hata olu�tu" });
            }
        }
    }
}