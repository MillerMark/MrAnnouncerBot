using System;
using System.Linq;
using System.Collections.Generic;
using Imaging;

namespace DHDM
{
    public class LightSequenceData : ILightSequenceData
    {
        /// <summary>
        /// The hue of the color, from 0 to 360.
        /// </summary>
        public int Hue { get; set; } = 0;

        /// <summary>
        /// The saturation of the color, from 0 to 100.
        /// </summary>
        public int Saturation { get; set; } = 100;

        /// <summary>
        /// The lightness of the color, from 0 to 100.
        /// </summary>
        public int Lightness { get; set; } = 0;

        /// <summary>
        /// The length of time in seconds this data is valid.
        /// </summary>
        public double Duration { get; set; } = 1d / 30d; // 30 fps
        
        string IColor.AsHtml => new HueSatLight(Hue / 360.0, Saturation / 100.0, Lightness / 100.0).AsHtml;

        public LightSequenceData()
        {

        }

        public LightSequenceData(int hue, int saturation, int lightness, double duration)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
            Duration = duration;
        }

        public bool SameColor(LightSequenceData lightSequenceData)
        {
            return Hue == lightSequenceData.Hue &&
                         Saturation == lightSequenceData.Saturation &&
                         Lightness == lightSequenceData.Lightness;
        }

        public IEnumerable<LightSequenceData> Decompress()
        {
            const double singleFrameDuration = 1d / 30d;
            const double halfFrameDuration = singleFrameDuration / 2d;
            List<LightSequenceData> result = new List<LightSequenceData>();
            double totalDuration = Duration;
            while (totalDuration >= halfFrameDuration)
            {
                LightSequenceData lightSequenceData = new LightSequenceData() { Hue = Hue, Saturation = Saturation, Lightness = Lightness, Duration = singleFrameDuration };
                result.Add(lightSequenceData);
                totalDuration -= singleFrameDuration;
            }
            return result;
        }

        public LightSequenceData Clone()
        {
            return new LightSequenceData(Hue, Saturation, Lightness, Duration);
        }

        public bool Matches(LightSequenceData currentValue)
        {
            return Hue == currentValue.Hue &&
                Saturation == currentValue.Saturation &&
                Lightness == currentValue.Lightness;
        }

        public void SetFrom(IColor? value)
        {
            if (value == null)
                return;
            Hue = value.Hue;
            Saturation = value.Saturation;
            Lightness = value.Lightness;
        }
    }
}
