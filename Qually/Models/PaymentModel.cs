using QuallyLib;

namespace Qually.Models
{
    public class PaymentModel
    {
        public SubModel SubModel { get; set; }
        public Order Order { get; set; }
        public string SignatureValue { get; set; }
    }
}
