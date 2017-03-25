using Models;

namespace EvotoClient.ViewModel
{
    public class CustomUserFieldViewModel : EvotoViewModelBase
    {
        private string _name;

        private bool _required;

        private string _value;

        public CustomUserFieldViewModel(CustomUserField model)
        {
            Name = model.Name;
            Required = model.Required;
        }

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        public bool Required
        {
            get { return _required; }
            set { Set(ref _required, value); }
        }
    }
}