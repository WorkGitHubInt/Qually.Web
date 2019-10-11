using System;
using System.ComponentModel.DataAnnotations;

namespace QuallyLib
{
    public enum PayType
    {
        Yandex,
        Qiwi,
        Robokassa
    }

    public class Transaction
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Не указан номер транзакции!")]
        public string Operation_Id { get; set; }
        [Required(ErrorMessage = "Не указана дата!")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Не указана сумма!")]
        public decimal Sum { get; set; }
        [Required(ErrorMessage = "Не указан id пользователя!")]
        public string UserId { get; set; }
        public PayType Type { get; set; }
        public string Sender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
