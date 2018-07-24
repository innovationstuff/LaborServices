using LaborServices.Entity;
using LaborServices.ViewModel;
using System.Collections.Generic;

namespace LaborServices.Managers.Mapping
{
   public static class ToModelConverter
    {

        /// <summary>
        /// Convert the ViewModel to Model
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <typeparam name="T1">ViewModel</typeparam>
        /// <param name="viewModel">viewModel you need to convert</param>
        /// <returns></returns>
        public static T ToModel<T, T1>(this T1 viewModel)
            where T : IEntityBase
            where T1 : IViewModel
        {
            return AutoMapper.Mapper.Map<T>(viewModel);
        }

        /// <summary>
        ///  Convert the list of ViewModel to list of Model
        /// </summary>
        /// <typeparam name="T">list of Model</typeparam>
        /// <typeparam name="T1">list of ViewModel</typeparam>
        /// <param name="viewModel">list of viewModel you need to convert</param>
        /// <returns></returns>
        public static List<T> ToModel<T, T1>(this List<T1> viewModel)
            where T : IEntityBase
            where T1 : IViewModel
        {
            return AutoMapper.Mapper.Map<List<T>>(viewModel);
        }
    }
}
