

using System.Collections.Generic;

namespace DefaultNamespace
{
    public class Stat
    {
        private float value;

        public float Value
        {
            get
            {
                float currentValue = value+flatsModifiers;
                currentValue +=currentValue* percentsModifiers*.01f ;
                return currentValue ;
            }
            set { this.value = value; }
        }
        private float maxValue;
        public float MaxValue => maxValue;

        private float percentsModifiers=0;
        private float flatsModifiers=0;

        public Stat(float value)
        {
            maxValue = value;
            this.value = value;
        }

        public void AddPercentModifier(float pourcent)
        {
            percentsModifiers+=pourcent;
        }

        public void AddFlatModifier(float ammount)
        {
            flatsModifiers+=ammount;
        }

        public void RemovePercentModifier(float pourcent)
        {
            percentsModifiers-=pourcent;
        }

        public void RemoveFlatModifier(float ammount)
        {
            flatsModifiers-=ammount;
        }
    }
}