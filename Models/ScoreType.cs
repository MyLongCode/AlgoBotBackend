using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlgoBotBackend.Models
{
    public enum ScoreType
    {
        [Display(Name = "Сумма")]
        Summa,
        [Display(Name = "Процент")]
        Procent
    }
}
