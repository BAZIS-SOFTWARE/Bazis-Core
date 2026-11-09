namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Анизотропия свойств материала («Модель материала»).
    /// </summary>
    public enum MaterialModel
    {
        /// <summary>
        /// Изотропный материал: скалярные теплопроводность, модуль Юнга и ТКЛР.
        /// </summary>
        Isotropic = 1,
        /// <summary>
        /// Ортотропный материал: тройки X/Y/Z для теплопроводности, модуля Юнга и ТКЛР.
        /// </summary>
        Orthotropic = 2
    }
}
