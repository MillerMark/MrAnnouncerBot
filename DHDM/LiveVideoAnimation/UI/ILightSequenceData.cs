using System;
using System.Linq;

namespace DHDM
{
    public class ColorHolder: IColor
    {
        public int Hue { get; set; }
        public int Lightness { get; set; }
        public int Saturation { get; set; }
        public string AsHtml { get; }
        public ColorHolder(int hue, int lightness, int saturation, string asHtml)
        {
            Hue = hue;
            Lightness = lightness;
            Saturation = saturation;
            AsHtml = asHtml;
        }
    }

    public interface IColor
    {
        int Hue { get; set; }
        int Lightness { get; set; }
        int Saturation { get; set; }
        string AsHtml { get; }
    }

    public interface ILightSequenceData: IColor
    {
        LightSequenceData Clone();
        IEnumerable<LightSequenceData> Decompress();
        bool Matches(LightSequenceData currentValue);
        bool SameColor(LightSequenceData lightSequenceData);
        void SetFrom(IColor value);

        /// <summary>
        /// The length of time in seconds this data is valid.
        /// </summary>
        double Duration { get; set; }
    }
}
