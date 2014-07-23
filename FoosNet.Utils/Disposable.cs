using System;

namespace FoosNet.Utils
{
    public abstract class Disposable : IDisposable
    {
        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}
