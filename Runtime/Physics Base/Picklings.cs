#nullable enable
using UnityEngine;

namespace UnityExtras
{
    #region Interfaces
    public interface IPickling<Pickling, in Self>
        where Pickling : IPickling<Self, Pickling>
        where Self : IPickling<Pickling, Self>
    {
        Pickling? connectedPickling { get; set; }
    }

    public interface IPicker<in Picker, PickUp> : IPickling<PickUp, Picker>
        where Picker : IPicker<Picker, PickUp>
        where PickUp : IPickUp<Picker, PickUp>
    {
        bool PickUpCast(out PickUp? pickUp);
    }

    public interface IPickUp<Picker, in PickUp> : IPickling<Picker, PickUp>
        where Picker : IPicker<Picker, PickUp>
        where PickUp : IPickUp<Picker, PickUp>
    {
        void OnHold(Picker picker);
        void OnDrop();
    }

    public static class IPickingExtensions
    {
        public static void Hold<Pickling, Self>(this IPickling<Pickling, Self> self, Pickling pickling)
            where Pickling : class, IPickling<Self, Pickling>
            where Self : class, IPickling<Pickling, Self>
        {
            if (self.connectedPickling == pickling)
            {
                return;
            }

            self.Drop();

            self.connectedPickling = pickling;
            pickling.Hold((Self)self);
        }

        public static void Drop<Pickling, Self>(this IPickling<Pickling, Self> self)
            where Pickling : class, IPickling<Self, Pickling>
            where Self : class, IPickling<Pickling, Self>
        {
            var tmp = self.connectedPickling;
            self.connectedPickling = null;

            if (tmp != null)
            {
                tmp.Drop();
            }
        }
    }

    public static class IPickerExtensions
    {
        /// <summary>Get the held <typeparam name="PickUp"/> or try to hold a <typeparam name="PickUp"/> using <see cref="IPicker{Picker, PickUp}.PickUpCast"/>.</summary>
        /// <param name="pickUp">The <typeparam name="PickUp"/> held or now being held.</param>
        /// <returns>If the <typeparam name="Picker"/> held or is now holding a <typeparam name="PickUp"/>.</returns>
        public static bool HeldOrTryHold<Picker, PickUp>(this IPicker<Picker, PickUp> picker, out PickUp? pickUp)
            where Picker : class, IPicker<Picker, PickUp>
            where PickUp : class, IPickUp<Picker, PickUp>
            => (pickUp = picker.connectedPickling) != null || picker.TryHold(out pickUp);

        /// <summary>Try to hold a <typeparamref name="PickUp"/> using <see cref="IPicker{Picker, PickUp}.PickUpCast"/>.</summary>
        /// <param name="pickUp">The <typeparamref name="PickUp"/> now being held.</param>
        /// <returns>If the <typeparamref name="Picker"/> is now holding a <typeparamref name="PickUp"/>.</returns>
        public static bool TryHold<Picker, PickUp>(this IPicker<Picker, PickUp> picker, out PickUp? pickUp)
            where Picker : class, IPicker<Picker, PickUp>
            where PickUp : class, IPickUp<Picker, PickUp>
        {
            bool result;
            if (result = picker.PickUpCast(out pickUp))
            {
                picker.Hold(pickUp!);
            }

            return result;
        }

        /// <summary>Hold a <typeparamref name="PickUp"/>.</summary>
        /// <param name="pickUp">The <typeparamref name="PickUp"/> to hold.</param>
        public static void Hold<Picker, PickUp>(this IPicker<Picker, PickUp> picker, PickUp pickUp)
            where Picker : class, IPicker<Picker, PickUp>
            where PickUp : class, IPickUp<Picker, PickUp>
        {
            var tmp = pickUp.connectedPickling;
            ((IPickling<PickUp, Picker>)picker).Hold(pickUp);
            if (tmp != picker)
            {
                pickUp.OnHold((Picker)picker);
            }
        }

        /// <summary>Drop the currently held <typeparamref name="PickUp"/>.</summary>
        /// <remarks><seealso cref="IPickling{Pickling, Self}.connectedPickling"/>.</remarks>
        public static void Drop<Picker, PickUp>(this IPicker<Picker, PickUp> picker)
            where Picker : class, IPicker<Picker, PickUp>
            where PickUp : class, IPickUp<Picker, PickUp>
        {
            if (picker.connectedPickling != null)
            {
                picker.connectedPickling.OnDrop();
            }
            ((IPickling<PickUp, Picker>)picker).Drop();
        }
    }

    public static class IPickUpExtensions
    {
        /// <summary>Be held by a <typeparamref name="Picker"/>.</summary>
        /// <param name="picker">The <typeparamref name="Picker"/> to be held by.</param>
        public static void Hold<Picker, PickUp>(this IPickUp<Picker, PickUp> pickUp, Picker picker)
            where Picker : class, IPicker<Picker, PickUp>
            where PickUp : class, IPickUp<Picker, PickUp>
        {
            var tmp = pickUp.connectedPickling;
            ((IPickling<Picker, PickUp>)pickUp).Hold(picker);
            if (tmp != picker)
            {
                pickUp.OnHold(picker);
            }
        }

        /// <summary>Be dropped by the currently held <typeparamref name="Picker"/>.</summary>
        /// <remarks><seealso cref="IPickling{Pickling, Self}.connectedPickling"/>.</remarks>
        public static void Drop<Picker, PickUp>(this IPickUp<Picker, PickUp> pickUp)
            where Picker : class, IPicker<Picker, PickUp>
            where PickUp : class, IPickUp<Picker, PickUp>
        {
            if (pickUp.connectedPickling != null)
            {
                pickUp.OnDrop();
            }
            ((IPickling<Picker, PickUp>)pickUp).Drop();
        }
    }
    #endregion

    #region Abstract Classes
    /// <summary>Caster to check for <typeparamref name="PickUp"/>.</summary>
    public abstract class PickerBase<Picker, PickUp> : MonoBehaviour, IPicker<Picker, PickUp>
        where Picker : class, IPicker<Picker, PickUp>
        where PickUp : class, IPickUp<Picker, PickUp>
    {
        PickUp? IPickling<PickUp, Picker>.connectedPickling { get => heldPickUp; set => heldPickUp = value; }

        public PickUp? heldPickUp { get; private set; }

        public abstract bool PickUpCast(out PickUp? pickUp);
    }

    public abstract class PickUpBase<Picker, PickUp> : MonoBehaviour, IPickUp<Picker, PickUp>
        where Picker : class, IPicker<Picker, PickUp>
        where PickUp : class, IPickUp<Picker, PickUp>
    {
        Picker? IPickling<Picker, PickUp>.connectedPickling { get => holdingPicker; set => holdingPicker = value; }

        public Picker? holdingPicker { get; private set; }

        protected virtual void OnDisable()
        {
            this.Drop();
        }

        public void Hold(Picker picker)
        {
            if (!enabled)
            {
                return;
            }

            ((IPickUp<Picker, PickUp>)this).Hold(picker);
        }

        protected abstract void OnHold(Picker picker);
        protected abstract void OnDrop();

        void IPickUp<Picker, PickUp>.OnHold(Picker picker) => OnHold(picker);
        void IPickUp<Picker, PickUp>.OnDrop() => OnDrop();
    }
    #endregion
}