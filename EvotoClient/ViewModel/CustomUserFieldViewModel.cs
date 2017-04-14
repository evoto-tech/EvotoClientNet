using System;
using Models;

namespace EvotoClient.ViewModel
{
    public class CustomUserFieldViewModel : EvotoViewModelBase
    {
        private readonly CustomUserField _model;

        private string _value;

        public CustomUserFieldViewModel(CustomUserField model)
        {
            _model = model;
        }

        public string Name => _model.Name;

        public bool Required => _model.Required;

        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        public DateTime? Date
        {
            get
            {
                try
                {
                    return DateTime.Parse(_value);
                }
                catch
                {
                    return null;
                }
            }
            set { Value = value?.ToString(); }
        }

        public bool ShowText => _model.Type != "Date";

        public bool ShowDate => _model.Type == "Date";

        public CustomUserField GetModel()
        {
            _model.Value = Value;
            return _model;
        }
    }
}