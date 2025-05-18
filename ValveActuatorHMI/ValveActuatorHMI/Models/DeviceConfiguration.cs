using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValveActuatorHMI.Models
{
    [Table("DeviceConfigurations")]
    public class DeviceConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string DeviceType { get; set; }

        [Required]
        public string ConfigurationJson { get; set; }
    }
}