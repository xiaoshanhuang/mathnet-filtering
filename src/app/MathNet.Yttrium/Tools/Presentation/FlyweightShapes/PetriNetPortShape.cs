using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using Netron.Diagramming.Core;
using MathNet.Symbolics.Presentation.Shapes;

namespace MathNet.Symbolics.Presentation.FlyweightShapes
{
    public class PetriNetPortShape : IFlyweightShape<PortShape>
    {
        public void InitShape(PortShape shape)
        {
            shape.AutoSize = false;

            if(shape.Port == null)
            {
                shape.Text = shape.PortReference.Index.ToString();
                shape.Width = 5;
                shape.Height = 20;
            }
            else
            {
                Port port = shape.Port;
                shape.Text = port.Entity.ToString();
                int inCnt = port.InputSignalCount;
                int outCnt = port.OutputSignalCount;
                int inoutCnt = Math.Max(inCnt, outCnt);
                int busCnt = port.BusCount;

                shape.Width = 6 + 2 * busCnt;
                shape.Height = 20 + 5 * inoutCnt;

                SyncConnectors(shape.Connectors, shape.Model, inCnt, shape.InputConnectors, "Input");
                SyncConnectors(shape.Connectors, shape.Model, outCnt, shape.OutputConnectors, "Output");
                SyncConnectors(shape.Connectors, shape.Model, busCnt, shape.BusConnectors, "Bus");

                Point loc = shape.Location;
                int right = loc.X + shape.Width;

                List<Connector> list = shape.InputConnectors;
                for(int i = 0; i < inCnt; i++)
                    list[i].Point = new Point(loc.X, loc.Y + 10 + i * 5);
                list = shape.OutputConnectors;
                for(int i = 0; i < outCnt; i++)
                    list[i].Point = new Point(right, loc.Y + 10 + i * 5);
                list = shape.BusConnectors;
                for(int i = 0; i < busCnt; i++)
                    list[i].Point = new Point(loc.X + 3 + 2 * i, loc.Y);
            }

            shape.Resizable = false;
            //shape.PaintStyle = new GradientPaintStyle(Color.Navy, Color.SteelBlue, -135);
            //shape.PaintStyle = new GradientPaintStyle(Color.SteelBlue, Color.LightBlue, -135);
            shape.PaintStyle = new GradientPaintStyle(Color.DarkGreen, Color.Yellow, -135);

            //Rectangle rect = shape.Rectangle;
            //GraphicsPath path = new GraphicsPath();
            //path.AddArc(rect.X, rect.Y, 20, 20, -180, 90);
            //path.AddLine(rect.X + 10, rect.Y, rect.X + rect.Width - 10, rect.Y);
            //path.AddArc(rect.X + rect.Width - 20, rect.Y, 20, 20, -90, 90);
            //path.AddLine(rect.X + rect.Width, rect.Y + 10, rect.X + rect.Width, rect.Y + rect.Height - 10);
            //path.AddArc(rect.X + rect.Width - 20, rect.Y + rect.Height - 20, 20, 20, 0, 90);
            //path.AddLine(rect.X + rect.Width - 10, rect.Y + rect.Height, rect.X + 10, rect.Y + rect.Height);
            //path.AddArc(rect.X, rect.Y + rect.Height - 20, 20, 20, 90, 90);
            //path.AddLine(rect.X, rect.Y + rect.Height - 10, rect.X, rect.Y + 10);

            //shape.Path = path;
        }

        public void Paint(PortShape shape, Graphics g)
        {
            Rectangle rect = shape.Rectangle;

            g.SmoothingMode = SmoothingMode.HighQuality;

            //the shadow
            g.FillRectangle(ArtPallet.ShadowBrush, rect.X + 3, rect.Y + 3, rect.Width, rect.Height);

            //the actual shape
            g.FillRectangle(shape.Brush, rect);

            //the edge of the bundle
            if(shape.Hovered || shape.IsSelected)
                g.DrawRectangle(ArtPallet.HighlightPen, rect);
            else
                g.DrawRectangle(shape.Pen, rect);

            //the connectors
            Brush cBrush = ArtPallet.GetSolidBrush(Color.Gray, 255); //Color.SteelBlue
            foreach(Connector c in shape.OutputConnectors)
            {
                //g.FillRectangle(cBrush, c.Point.X - 7, c.Point.Y - 12, 7, 24);
                //g.FillPie(cBrush, c.Point.X - 18, c.Point.Y - 12, 24, 24, 90, 180);
                //g.FillEllipse(cBrush, c.Point.X - 12, c.Point.Y - 12, 24, 24);
            }
            foreach(Connector c in shape.Connectors)
                c.Paint(g);
        }

        private void SyncConnectors(ICollection<IConnector> connectors, IModel model, int count, List<Connector> list, string prefix)
        {
            if(count > list.Count)
                for(int i = list.Count; i < count; i++)
                {
                    Connector c = new Connector(model);
                    c.Name = prefix + i.ToString();
                    c.Parent = this;
                    list.Add(c);
                    connectors.Add(c);
                }
            else if(count < list.Count)
                for(int i = list.Count - 1; i > count; i--)
                {
                    Connector c = list[i];
                    connectors.Remove(c);
                    list.RemoveAt(i);
                }
        }
    }
}