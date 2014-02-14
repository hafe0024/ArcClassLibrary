using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Media;

namespace Enbridge.DepthOfCover
{
    /// <summary>
    /// 
    /// </summary>
    public static class DOC_Renderer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ClassBreaksRenderer MakeDOCRenderer()
        {
            double symbolSize = 10;

            ClassBreaksRenderer breaksRenderer = new ClassBreaksRenderer();
            breaksRenderer.Field = "Measurement";
            SimpleMarkerSymbol defaultSymbol = new SimpleMarkerSymbol();
            defaultSymbol.Size = symbolSize;
            defaultSymbol.Color = (SolidColorBrush)new BrushConverter().ConvertFromString("#f50404");
            defaultSymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
            breaksRenderer.DefaultSymbol = defaultSymbol;

            List<double> min = new List<double>
            {
                0,6,12,18,24,30,36,48,60
            };

            List<double> max = new List<double>
            {
                6,12,18,24,30,36,48,60,1000
            };
            List<string> colors = new List<string>
            {
                "#fa5903","#fc8b00","#fcc304","#f5f502","#c7f704","#95f703","#5bf600","#02f502","#39a801"
            };

            for (int i = 0; i < 9; i++)
            {
                ESRI.ArcGIS.Client.ClassBreakInfo classBreak = new ESRI.ArcGIS.Client.ClassBreakInfo();

                SimpleMarkerSymbol breakSymbol = new SimpleMarkerSymbol();
                breakSymbol.Size = symbolSize;
                breakSymbol.Color = (SolidColorBrush)new BrushConverter().ConvertFromString(colors[i]);
                breakSymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;

                classBreak.Symbol = breakSymbol;
                classBreak.MinimumValue = min[i];
                classBreak.MaximumValue = max[i];

                breaksRenderer.Classes.Add(classBreak);

            }

            return breaksRenderer;
        }
    }
}

