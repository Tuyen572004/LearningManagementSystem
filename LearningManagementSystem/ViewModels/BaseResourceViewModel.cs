using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using System;
using System.Windows.Input;

namespace LearningManagementSystem.ViewModels
{
    public class BaseResourceViewModel : BaseViewModel
    {
        public BaseResource BaseResource { get; set; }

        public ICommand DeleteCommand { get; }

        private readonly UserService _userService = new UserService();

        public BaseResourceViewModel()
        {
            BaseResource = new BaseResource();
            DeleteCommand = new RelayCommand(Delete,CanDelete);
        }

        public BaseResourceViewModel(BaseResource x)
        {
            BaseResource = x;
            DeleteCommand = new RelayCommand(Delete, CanDelete);
        }

        private bool CanDelete()
        {
            return _userService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Teacher));
        }

        private void Delete()
        {
            WeakReferenceMessenger.Default.Send(new DeleteMessage(this));
        }

    }
}
