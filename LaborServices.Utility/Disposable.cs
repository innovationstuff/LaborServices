﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Utility
{
    public class Disposable : IDisposable
    {
        private bool isDisposed;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                DisposeCore();
            }

            isDisposed = true;
        }

        // override this to dispose custom objects
        protected virtual void DisposeCore()
        {
        }
    }
}