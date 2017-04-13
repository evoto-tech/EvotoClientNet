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
            => "Ends: " + _model.ExpiryDate.ToShortTimeString() + " " + _model.ExpiryDate.ToShortDateString();

        public BlockchainDetails GetModel()
        {
            return _model;
        }
    }
}