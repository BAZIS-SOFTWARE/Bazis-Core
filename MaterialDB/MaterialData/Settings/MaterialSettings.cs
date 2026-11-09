namespace MaterialDB.MaterialData.Settings
{
    /// <summary>
    /// Настройки, от которых зависит состав свойств материала любой среды.
    /// </summary>
    public class MaterialSettings
    {
        /// <summary>
        /// «Модель материала». Ортотропный материал получает тройки X/Y/Z вместо скалярных свойств.
        /// </summary>
        public MaterialModel MaterialModel { get; set; } = MaterialModel.Isotropic;
    }
}
