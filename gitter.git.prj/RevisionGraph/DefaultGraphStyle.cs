﻿namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;

	using gitter.Framework;

	public sealed class DefaultGraphStyle : IGraphStyle
	{
		private const float HoverLightAmount = 0.4f;

		private static readonly Brush TagBrush = new SolidBrush(ColorScheme.TagBackColor);
		private static readonly Brush LocalBranchBrush = new SolidBrush(ColorScheme.LocalBranchBackColor);
		private static readonly Brush RemoteBranchBrush = new SolidBrush(ColorScheme.RemoteBranchBackColor);
		private static readonly Brush StashBrush = new SolidBrush(ColorScheme.StashBackColor);

		private static readonly Brush TagBrushHovered = new SolidBrush(ColorScheme.TagBackColor.Lighter(HoverLightAmount));
		private static readonly Brush LocalBranchBrushHovered = new SolidBrush(ColorScheme.LocalBranchBackColor.Lighter(HoverLightAmount));
		private static readonly Brush RemoteBranchBrushHovered = new SolidBrush(ColorScheme.RemoteBranchBackColor.Lighter(HoverLightAmount));
		private static readonly Brush StashBrushHovered = new SolidBrush(ColorScheme.StashBackColor.Lighter(HoverLightAmount));

		private static readonly Pen TagBorderPen = SystemPens.InfoText;
		private static readonly Pen CirclePen = new Pen(Color.Black, 1.5f);

		private static readonly Point[] TagPoints = new Point[6];
		private static readonly PointF[] CurrentIndicator = new PointF[3];

		public void DrawGraph(Graphics gx, GraphAtom[] graphLine, Rectangle rect, int cellWidth, RevisionGraphItemType type, bool useColors)
		{
			if(gx == null) throw new ArgumentNullException("gx");
			if(graphLine != null)
			{
				int x = rect.X;
				int y = rect.Y;
				int w = cellWidth;
				int hw = w / 2;
				int h = rect.Height;
				int hh = h / 2;
				int r = rect.Right;
				for(int i = 0; (i < graphLine.Length) && (x < r); ++i, x += w)
				{
					var atom = graphLine[i];
					var colors = atom.ElementColors;

					if(!atom.IsEmpty)
					{
						if(atom.HasElement(GraphElement.VerticalTop))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.VerticalTop] : 0];
							gx.DrawLine(pen, x + hw, y, x + hw, y + hh);
						}
						if(atom.HasElement(GraphElement.VerticalBottom))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.VerticalBottom] : 0];
							gx.DrawLine(pen, x + hw, y + hh, x + hw, y + h);
						}
						if(atom.HasElement(GraphElement.HorizontalLeft))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.HorizontalLeft] : 0];
							gx.DrawLine(pen, x, y + hh, x + hw, y + hh);
						}
						if(atom.HasElement(GraphElement.HorizontalRight))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.HorizontalRight] : 0];
							gx.DrawLine(pen, x + hw, y + hh, x + w, y + hh);
						}
						if(atom.HasElement(GraphElement.LeftTopCorner))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.LeftTopCorner] : 0];
							gx.DrawLine(pen, x, y + hh, x + hw, y);
						}
						if(atom.HasElement(GraphElement.LeftTop))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.LeftTop] : 0];
							gx.DrawLine(pen, x + hw, y + hh, x, y);
						}
						if(atom.HasElement(GraphElement.RightTopCorner))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.RightTopCorner] : 0];
							gx.DrawLine(pen, x + w, y + hh, x + hw, y);
						}
						if(atom.HasElement(GraphElement.RightTop))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.RightTop] : 0];
							gx.DrawLine(pen, x + hw, y + hh, x + w, y);
						}
						if(atom.HasElement(GraphElement.RightBottomCorner))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.RightBottomCorner] : 0];
							gx.DrawLine(pen, x + w, y + hh, x + hw, y + h);
						}
						if(atom.HasElement(GraphElement.RightBottom))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.RightBottom] : 0];
							gx.DrawLine(pen, x + hw, y + hh, x + w, y + h);
						}
						if(atom.HasElement(GraphElement.LeftBottomCorner))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.LeftBottomCorner] : 0];
							gx.DrawLine(pen, x, y + hh, x + hw, y + h);
						}
						if(atom.HasElement( GraphElement.LeftBottom))
						{
							var pen = GraphColors.Pens[useColors ? colors[GraphElementId.LeftBottom] : 0];
							gx.DrawLine(pen, x + hw, y + hh, x, y + h);
						}
						if(atom.HasElement(GraphElement.Dot))
						{
							const int cr = 3;
							const int cd = cr * 2;
							int cx = x + hw - cr;
							int cy = y + hh - cr;
							switch(type)
							{
								case RevisionGraphItemType.Generic:
									{
										var brush = Brushes.Black;
										gx.FillEllipse(brush, cx, cy, cd, cd);
									}
									break;
								case RevisionGraphItemType.Current:
									{
										var brush = Brushes.LightGreen;
										gx.FillEllipse(brush, cx, cy, cd, cd);
										var pen = CirclePen;
										gx.DrawEllipse(pen, cx + .5f, cy + .5f, cd - 1, cd - 1);
									}
									break;
								case RevisionGraphItemType.Uncommitted:
									{
										var brush = SystemBrushes.Window;
										gx.FillEllipse(brush, cx, cy, cd, cd);
										var pen = CirclePen;
										gx.DrawEllipse(pen, cx + .5f, cy + .5f, cd - 1, cd - 1);
									}
									break;
								case RevisionGraphItemType.Unstaged:
									{
										var brush = Brushes.Red;
										gx.FillEllipse(brush, cx, cy, cd, cd);
										var pen = CirclePen;
										gx.DrawEllipse(pen, cx + .5f, cy + .5f, cd - 1, cd - 1);
									}
									break;
							}
						}
					}
				}
			}
		}

		public void DrawReferenceConnector(Graphics gx, GraphAtom[] graphLine, int graphX, int cellWidth, int refX, int y, int h)
		{
			int revisionPos = -1;
			for(int i = 0; i < graphLine.Length; ++i)
			{
				if((graphLine[i].Elements & GraphElement.Dot) == GraphElement.Dot)
				{
					revisionPos = i;
					break;
				}
			}
			if(revisionPos != -1)
			{
				int cx = graphX + revisionPos * cellWidth + cellWidth / 2;
				int cy = y + h / 2;
				gx.DrawEllipse(TagBorderPen, cx - 5, cy - 5, 10, 10);
				gx.DrawLine(TagBorderPen, cx + 6, cy, refX, cy);
			}
		}

		public void DrawReferencePresenceIndicator(Graphics gx, GraphAtom[] graphLine, int graphX, int cellWidth, int y, int h)
		{
			int revisionPos = -1;
			for(int i = 0; i < graphLine.Length; ++i)
			{
				if((graphLine[i].Elements & GraphElement.Dot) == GraphElement.Dot)
				{
					revisionPos = i;
					break;
				}
			}
			if(revisionPos != -1)
			{
				int cx = graphX + revisionPos * cellWidth + cellWidth / 2;
				int cy = y + h / 2;
				gx.DrawEllipse(TagBorderPen, cx - 5, cy - 5, 10, 10);
			}
		}

		private static void SetTagPoints(int x, int y, int w, int h)
		{
			int d = (h + 1) / 2;
			int vc = y + d;
			TagPoints[0] = new Point(x, vc + 1);
			TagPoints[1] = new Point(x, vc - 1);
			TagPoints[2] = new Point(x + d - 1, y);
			TagPoints[3] = new Point(x + w, y);
			TagPoints[4] = new Point(x + w, y + h);
			TagPoints[5] = new Point(x + d - 1, y + h);
		}

		private static int DrawInlineTag(Graphics gx, Font font, Brush backBrush, StringFormat format, string text, int x, int y, int right, int h)
		{
			var size = GitterApplication.TextRenderer.MeasureText(
				gx, text, font, right - x, format);
			int w = size.Width + 1 + 6;
			int texth = size.Height;
			right -= 4;
			if(w > right - x)
				w = right - x;
			if(w <= 5) return 0;
			SetTagPoints(x, y + 2, w + 4, h - 5);
			gx.FillPolygon(backBrush, TagPoints);
			gx.DrawPolygon(TagBorderPen, TagPoints);
			if(w >= 7)
			{
				int d = (h - texth) / 2;
				var rc = new Rectangle(x + 4 + 3, y + d, w - 6, h - d);
				GitterApplication.TextRenderer.DrawText(
					gx, text, font, SystemBrushes.WindowText, rc, format);
			}
			return w + 4;
		}

		public int DrawTag(Graphics gx, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, Tag tag)
		{
			int w = DrawInlineTag(gx, font, hovered?TagBrushHovered:TagBrush, format, tag.Name, x, y, right, h);
			if(tag.TagType == TagType.Annotated)
				gx.DrawLine(TagBorderPen, x + w - 4, y + 3, x + w, y + 7);
			return w;
		}

		public int DrawBranch(Graphics gx, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, BranchBase branch)
		{
			if(branch.IsRemote)
			{
				int w = DrawInlineTag(gx, font, hovered?RemoteBranchBrushHovered:RemoteBranchBrush, format, branch.Name, x, y, right, h);
				return w;
			}
			else
			{
				int w = DrawInlineTag(gx, font, hovered?LocalBranchBrushHovered:LocalBranchBrush, format, branch.Name, x, y, right, h);
				if(branch.IsCurrent)
				{
					CurrentIndicator[0].X = x + 1.5f;
					CurrentIndicator[0].Y = y + h / 2;
					CurrentIndicator[1].X = x + 1.5f + 4;
					CurrentIndicator[1].Y = y + h / 2 - 4.5f;
					CurrentIndicator[2].X = x + 1.5f + 4;
					CurrentIndicator[2].Y = y + h / 2 + 4.5f;
					gx.FillPolygon(SystemBrushes.InfoText, CurrentIndicator);
				}
				return w;
			}
		}

		public int DrawStash(Graphics gx, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, StashedState stash)
		{
			int w = DrawInlineTag(gx, font, hovered?StashBrushHovered:StashBrush, format, GitConstants.StashName, x, y, right, h);
			return w;
		}

		private int MeasureInlineTag(Graphics gx, Font font, StringFormat format, string text)
		{
			var size = GitterApplication.TextRenderer.MeasureText(
				gx, text, font, int.MaxValue, format);
			int w = size.Width + 1 + 6;
			int texth = size.Height;
			return w + 5;
		}

		public int MeasureTag(Graphics gx, Font font, StringFormat format, Tag tag)
		{
			return MeasureInlineTag(gx, font, format, tag.Name);
		}

		public int MeasureBranch(Graphics gx, Font font, StringFormat format, Branch branch)
		{
			return MeasureInlineTag(gx, font, format, branch.Name);
		}

		public int MeasureStash(Graphics gx, Font font, StringFormat format, StashedState stash)
		{
			return MeasureInlineTag(gx, font, format, GitConstants.StashName);
		}
	}
}
