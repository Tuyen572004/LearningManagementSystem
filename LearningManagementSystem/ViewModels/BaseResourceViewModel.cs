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
    public class BaseResourceViewModel : BaseViewModel
    {
        public BaseResource BaseResource { get; set; }

        public ICommand DeleteCommand { get; }

        private readonly UserService _userService = new UserService();

        public readonly bool CanEdit;

        public BaseResourceViewModel()
        {
            BaseResource = new BaseResource();
            DeleteCommand = new RelayCommand(Delete,CanDelete);
            CanEdit = !UserService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Student));
        }

        public BaseResourceViewModel(BaseResource x)
        {
            BaseResource = x;
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            CanEdit = !UserService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Student));
        }

        private bool CanDelete()
        {
            return CanEdit;
        }

        private void Delete()
        {
            WeakReferenceMessenger.Default.Send(new DeleteResourceMessage(this));
        }

    }
}
