﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeMakerApi;

namespace CafeteiraEletrica
{
    public class M4RecipienteDeContencao : RecipienteDeContencao, IPrepararCafe
    {
        private ICoffeeMakerApi _api;

        public M4RecipienteDeContencao(ICoffeeMakerApi api)
        {
            _api = api;
        }

        protected internal override bool EstaPronto
        {
            get
            {
                return _api.GetWarmerPlateStatus() == WarmerPlateStatus.POT_EMPTY;
            }
        }

        internal override void Prepare()
        {
            EstaPreparando = true;
            _api.SetWarmerState(WarmerState.ON);
        }

        public void Preparando()
        {
            RecipienteDeContencaoRemovido();
            RecipienteDeContencaoDevolvido();
            CafeFoiConsumido();
        }


        private protected override void RecipienteDeContencaoRemovido()
        {
            if (EstaPreparando && _api.GetWarmerPlateStatus() != WarmerPlateStatus.WARMER_EMPTY) return;
            EstaPreparando = false;
            _api.SetWarmerState(WarmerState.OFF);
            InterrompaProducao();
        }
        private protected override void RecipienteDeContencaoDevolvido()
        {
            if (!EstaPreparando && _api.GetWarmerPlateStatus() == WarmerPlateStatus.WARMER_EMPTY) return;
            EstaPreparando = true;
            _api.SetWarmerState(WarmerState.ON);
            RetorneProducao();
        }

        internal override void Pronto()
        {
            _api.SetWarmerState(WarmerState.ON);
            EstaPreparando = true;
            MensagemPronto();
        }
        private void CafeFoiConsumido()
        {
            if (_api.GetBrewButtonStatus() != BrewButtonStatus.NOT_PUSHED || _api.GetWarmerPlateStatus() != WarmerPlateStatus.POT_EMPTY) return;
            _api.SetWarmerState(WarmerState.ON);
            InterrompaProducao();
            MensagemPronto();
        }
    }
}
