using Models;

namespace EvotoClient.ViewModel
{
    public class CustomUserFieldViewModel : EvotoViewModelBase
    {
        private readonly CustomUserField _model;

        public CustomUserFieldViewModel(CustomUserField model)
        {
            _model = model;
        }

        public string Name => _model.Name;

        public bool Required => _model.Required;

        private string _value;
        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        public CustomUserField GetModel()
        {
            _model.Value = Value;
            return _model;
        }
    }
}