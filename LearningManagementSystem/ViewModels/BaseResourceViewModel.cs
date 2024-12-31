using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services.UserService;
using System;
using System.Windows.Input;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing base resources.
    /// </summary>
    public class BaseResourceViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the base resource.
        /// </summary>
        public BaseResource BaseResource { get; set; }

        /// <summary>
        /// Gets the command to delete the resource.
        /// </summary>
        public ICommand DeleteCommand { get; }

        private readonly UserService _userService = new UserService();

        /// <summary>
        /// Indicates whether the current user can edit the resource.
        /// </summary>
        public readonly bool CanEdit;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResourceViewModel"/> class.
        /// </summary>
        public BaseResourceViewModel()
        {
            BaseResource = new BaseResource();
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            CanEdit = !UserService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Student));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResourceViewModel"/> class with a specified resource.
        /// </summary>
        /// <param name="x">The base resource to initialize with.</param>
        public BaseResourceViewModel(BaseResource x)
        {
            BaseResource = x;
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            CanEdit = !UserService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Student));
        }

        /// <summary>
        /// Determines whether the resource can be deleted.
        /// </summary>
        /// <returns><c>true</c> if the resource can be deleted; otherwise, <c>false</c>.</returns>
        private bool CanDelete()
        {
            return CanEdit;
        }

        /// <summary>
        /// Deletes the resource.
        /// </summary>
        private void Delete()
        {
            WeakReferenceMessenger.Default.Send(new DeleteResourceMessage(this));
        }
    }
}
