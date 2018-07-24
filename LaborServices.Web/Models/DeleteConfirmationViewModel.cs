namespace LaborServices.Web.Models
{
    public class DeleteConfirmationViewModel
    {
        /// <summary>
        /// Action to perform after user confirms "Delete".
        /// </summary>
        public string PostDeleteAction { get; set; }

        /// <summary>
        /// [OPTIONAL] Controller to look for Post Delete action.
        /// This controller has implementation of Post Delete action
        /// </summary>
        public string PostDeleteController { get; set; }

        /// <summary>
        /// While executing POST Delete action, we need id of entity to delete.
        /// </summary>
        public int DeleteEntityId { get; set; }

        /// <summary>
        /// Delete Confirmation dialog header text. For example
        /// For text like "Delete Estimated Effort", Header Text is "Estimated Effort"
        /// </summary>
        public string HeaderText { get; set; }
    }
}