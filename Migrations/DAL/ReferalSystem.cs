using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlgoBotBackend.Migrations.DAL
{
	public enum ReferalSystem
	{
        [Display(Name = "Одноуровневая")]
        OneLevel,
        [Display(Name = "Двухуровневая")]
        TwoLevel,
        [Display(Name = "Трёхуровневая")]
        ThreeLevel
	}
}
