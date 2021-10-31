using System.Collections.Generic;
using System.Windows.Controls;

namespace ExtremumScan
{
    public class RBSelector<T>
    {
        private List<RadioButton> radioButtons;
        private List<T> choices;

        public RBSelector(List<RadioButton> rb, List<T> ch)
        {
            radioButtons = rb;
            choices = ch;
        }

        public T GetChoice()
        {
            for (int i = 0; i < radioButtons.Count; i++)
            {
                if ((bool)radioButtons[i].IsChecked)
                {
                    return choices[i];
                }
            }
            return default(T);
        }
    }
}