namespace BlockScanner.Rendering.MultiSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BaseMultiSourceRenderer<T> : BaseRenderer<T>, IMultiSourceRenderer, IDisposable
    {
        private readonly IList<IScannerSlot> slots = new List<IScannerSlot>();

        public BaseMultiSourceRenderer()
        {
        }

        public IEnumerable<IScannerSlot> ScannerSlots => slots;

        public IEnumerable<IScannerSlot> EmptyScannerSlots => slots.Where(s => s.IsEmpty);

        protected void AddSlot(IScannerSlot slot)
        {
            slots.Add(slot);
        }

        public virtual void Dispose()
        {
            foreach (var slot in ScannerSlots)
            {
                slot.Dispose();
            }
        }
    }
}
