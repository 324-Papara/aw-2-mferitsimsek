using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.Business.Dtos
{
    public class CustomerDetailDto
    {
        public int Id { get; set; }
        public string InsertUser { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsActive { get; set; }
        public long CustomerId { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string EducationStatus { get; set; }
        public string MontlyIncome { get; set; }
        public string Occupation { get; set; }
    }
}
