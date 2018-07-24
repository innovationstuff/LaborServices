using LaborServices.Entity;
using LaborServices.ViewModel;
using System.Collections.Generic;

namespace LaborServices.Managers.Mapping
{
   public static class ToViewModelConverter
    {
        /// <summary>
        /// Convert the Model to ViewModel
        /// </summary>
        /// <typeparam name="T">ViewModel</typeparam>
        /// <typeparam name="T1">Model</typeparam>
        /// <param name="model">model You need to convert</param>
        /// <returns></returns>
        public static T ToViewModel<T, T1>(this T1 model)
            where T : IViewModel
            where T1 : IEntityBase
        {
            return AutoMapper.Mapper.Map<T>(model);
        }

        /// <summary>
        /// Convert the list of Models to list of ViewModel
        /// </summary>
        /// <typeparam name="T">list of ViewModel</typeparam>
        /// <typeparam name="T1">list of Model</typeparam>
        /// <param name="model">list models You need to convert</param>
        /// <returns></returns>
        public static List<T> ToViewModel<T, T1>(this List<T1> model)
            where T : IViewModel
            where T1 : IEntityBase
        {
            return AutoMapper.Mapper.Map<List<T>>(model);
        }
    }
}
