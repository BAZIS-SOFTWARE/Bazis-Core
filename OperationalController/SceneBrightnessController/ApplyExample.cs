using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.SceneBrightnessController
{
    internal class ApplyDim
    {
        /// <summary>
        /// Применяет затемнение и лёгкую десатурацию ко всем объектам указанного типа, за исключением объектов, помеченных как выделенные.
        /// </summary>
        /// <param name="objType">Тип объектов, к которым будет применено затемнение.</param>
        private void ApplyDim(ObjType objType)
        {
            var modelObjs = project.GetModelObjects(objType).Where(obj => obj.Color != settingsConfig.SelectObjectColor);

            foreach (var item in modelObjs)
                item.Color = ApplyBrightness(item.Color, 0.6f, 0.85f);
        }

        /// <summary>
        /// Применяет затемнение и частичную десатурацию к одному объекту указанного типа по его номеру.
        /// </summary>
        /// <param name="objType">Тип объекта, к которому будет применено затемнение.</param>
        /// <param name="number">Номер (идентификатор) объекта в коллекции данного типа.</param>
        private void ApplyDimElement(ObjType objType, int number)
        {
            var modelObj = project.GetModelObject(objType, number);
            modelObj.Color = ApplyBrightness(modelObj.Color, 0.6f, 0.85f);
        }
    }
}
