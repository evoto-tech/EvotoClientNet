using System;
using Models;

namespace EvotoClient.ViewModel
{
    public class BlockchainViewModel : EvotoViewModelBase
    {
        private readonly BlockchainDetails _model;

        public BlockchainViewModel(BlockchainDetails model)
        {
            _model = model;
        }

        public string Name => _model.Name;

        public string Info => _model.Info;

        public string ChainString => _model.ChainString;

        public string EndDate
        {
            get
            {
                var time = _model.ExpiryDate.ToShortTimeString();
                var date = _model.ExpiryDate.ToShortDateString();
                var verb = (IsCurrent) ? "Ends" : "Ended";
                return $"{verb}: {time} {date}";
            }
        }

        public DateTime ExpiryDate => _model.ExpiryDate;

        public bool IsCurrent => _model.ExpiryDate > DateTime.UtcNow;

        public bool Encrypted
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_model.EncryptKey);
            }
        }

        public BlockchainDetails GetModel()
        {
            return _model;
        }
    }
}