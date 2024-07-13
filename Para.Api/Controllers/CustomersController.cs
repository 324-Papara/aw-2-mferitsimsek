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
                // Müþteri ile iliþkili adres , telefon ve detay bilgileriyle birlikte getiriyoruz.
                var customersWithDetails = await _unitOfWork.CustomerRepository.GetWithInclude(
                    c => c.CustomerAddresses,
                    c => c.CustomerPhones,
                    c => c.CustomerDetail
                );


                if (customersWithDetails == null || !customersWithDetails.Any())
                    return NotFound(new { Message = "Müþteri bilgisi bulunamadý" });

                return Ok(customersWithDetails);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapýlabilir..
                 _logger.LogError(ex, "Müþteri bilgileri getirilirken bir hata oluþtu");
                return StatusCode(500, new { Message = "Müþteri bilgileri getirilirken bir hata oluþtu" });
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

                // Burada yapýlan iþlem veritabanýndaki sorgu karþýlýðý aþaðýdaki " Örnek " gibidir
                #region Örnek
                    //dbContext.Customers
                    //    .Include(c => c.CustomerAddresses)
                    //    .Include(c => c.CustomerPhones)
                    //    .Include(c => c.CustomerDetail)
                    //    .ToListAsync();
                #endregion

                if (customer == null || !customer.Any())
                {
                    _logger.LogWarning("Belirtilen ID'ye sahip müþteri bulunamadý: {CustomerId}", customerId);
                    return NotFound(new { Message = $"ID: {customerId} olan müþteri bulunamadý" });
                }

                _logger.LogInformation("Müþteri baþarýyla getirildi. ID: {CustomerId}", customerId);
                return Ok(customer.First());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müþteri bilgisi getirilirken bir hata oluþtu. ID: {CustomerId}", customerId);
                return StatusCode(500, new { Message = "Müþteri bilgisi getirilirken bir hata oluþtu" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchCustomers(string propertyName, string comparison, string value)
        {
            try
            {
                // Dinamik sorgu ile müþterileri getiriyoruz.
                var customers = await _unitOfWork.CustomerRepository.GetWithDynamicQuery(
                    propertyName,
                    comparison,
                    value,
                    c => c.CustomerAddresses,
                    c => c.CustomerPhones
                );

                if (!customers.Any())
                {
                    _logger.LogWarning("Müþteri bulunamadý: {PropertyName} {Comparison} {Value}", propertyName, comparison, value);
                    return NotFound(new { Message = "Arama kriterlerine uygun müþteri bulunamadý" });
                }

                _logger.LogInformation("Müþteriler getirildi. Sayý: {Count}", customers.Count);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müþteri aramada hata");
                return StatusCode(500, new { Message = "Müþteri arama sýrasýnda bir hata oluþtu" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchCustomer([FromBody]CustomerSearchParams searchParams)
        {
            try
            {
                var customerQuery = await _unitOfWork.CustomerRepository.Where(c => true); // Baþlangýçta tüm müþteriler

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
                // Diðer arama kriterleri buraya eklenebilir ve daha dinamik hale getirilebilir

                var customers = await customerQuery
                    .Include(c => c.CustomerAddresses)
                    .Include(c => c.CustomerPhones)
                    .Include(c => c.CustomerDetail)
                    .ToListAsync();

                if (!customers.Any())
                {
                    _logger.LogInformation("Arama kriterlerine uygun müþteri bulunamadý.");
                    return NotFound(new { Message = "Arama kriterlerine uygun müþteri bulunamadý." });
                }

                _logger.LogInformation("Müþteriler baþarýyla arandý ve getirildi. Bulunan müþteri sayýsý: {Count}", customers.Count);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müþteri arama iþlemi sýrasýnda bir hata oluþtu.");
                return StatusCode(500, new { Message = "Müþteri arama iþlemi sýrasýnda bir hata oluþtu" });
            }
        }

       [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerDto customerDto)
        {
            try
            {
                // Dto olarak veriyi alýyoruz çünkü iliþkili tablolar var hataya düþmemek için.
                // Automapper ile mapleyerek veritabanýna gönderiryoruz.
                var customer = _mapper.Map<Customer>(customerDto);
                await _unitOfWork.CustomerRepository.Insert(customer);
                await _unitOfWork.Complete();

                _logger.LogInformation("Yeni müþteri baþarýyla eklendi. ID: {CustomerId}", customer.Id);
                return Ok(new { Message = "Müþteri baþarýyla eklendi", CustomerId = customer.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müþteri eklenirken bir hata oluþtu");
                return StatusCode(500, new { Message = "Müþteri eklenirken bir hata oluþtu" });
            }
        }

        [HttpPut("{customerId}")]
        public async Task<IActionResult> Put(long customerId, [FromBody] CustomerDto customerDto)
        {
            try
            {
                if (customerId != customerDto.Id)
                {
                    return BadRequest(new { Message = "URL'deki ID ile gönderilen müþteri ID'si uyuþmuyor" });
                }
                var customer = _mapper.Map<Customer>(customerDto);
                await _unitOfWork.CustomerRepository.Update(customer);
                await _unitOfWork.Complete();

                _logger.LogInformation("Müþteri baþarýyla güncellendi. ID: {CustomerId}", customerId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müþteri güncellenirken bir hata oluþtu. ID: {CustomerId}", customerId);
                return StatusCode(500, new { Message = "Müþteri güncellenirken bir hata oluþtu" });
            }
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> Delete(long customerId)
        {
            try
            {
                await _unitOfWork.CustomerRepository.Delete(customerId);
                await _unitOfWork.Complete();

                _logger.LogInformation("Müþteri baþarýyla silindi. ID: {CustomerId}", customerId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müþteri silinirken bir hata oluþtu. ID: {CustomerId}", customerId);
                return StatusCode(500, new { Message = "Müþteri silinirken bir hata oluþtu" });
            }
        }
    }
}