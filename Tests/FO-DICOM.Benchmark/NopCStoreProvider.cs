﻿using System;
using System.Text;
using System.Threading.Tasks;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Log;
using FellowOakDicom.Network;

namespace FellowOakDicom.Benchmark
{
    public class NopCStoreProvider : DicomService, IDicomServiceProvider, IDicomCStoreProvider
    {
        public NopCStoreProvider(INetworkStream stream, Encoding fallbackEncoding, Logger log,
            ILogManager logManager, INetworkManager networkManager,
            ITranscoderManager transcoderManager) : base(stream, fallbackEncoding, log,
            logManager, networkManager, transcoderManager)
        {
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
        }

        public void OnConnectionClosed(Exception exception)
        {
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            foreach (var presentationContext in association.PresentationContexts)
            {
                foreach (var ts in presentationContext.GetTransferSyntaxes())
                {
                    presentationContext.SetResult(DicomPresentationContextResult.Accept, ts);
                    break;
                }
            }

            return SendAssociationAcceptAsync(association);
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            return SendAssociationReleaseResponseAsync();
        }

        public Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
            => Task.FromResult(new DicomCStoreResponse(request, DicomStatus.Success));

        public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception e)
            => Task.CompletedTask;

    }
}
