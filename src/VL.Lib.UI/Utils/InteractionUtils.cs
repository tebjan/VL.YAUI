using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VL.Lib.IO.Notifications;
using VL.Lib.UI.Utils;

namespace VL.Lib.UI
{
    public static class Handler
    {
        private static Vector2 DoubleClickSize = new Vector2(1);

        internal class ContinuationMouseHandler : IMouseHandler
        {
            readonly Func<MouseNotification, IMouseHandler> continuation;

            public ContinuationMouseHandler(IMouseHandler handler, Func<MouseNotification, IMouseHandler> continuation)
            {
                Handler = handler;
                this.continuation = continuation;
            }

            public IMouseHandler Handler
            {
                get { return this.handler; }
                set
                {
                    this.handler = value;
                }
            }
            IMouseHandler handler;

            public IMouseHandler MouseDown(MouseDownNotification arg)
            {
                if (Handler != null)
                    Handler = Handler.MouseDown(arg);
                if (Handler == null)
                    return this.continuation(arg);
                return this;
            }

            public IMouseHandler MouseMove(MouseMoveNotification arg)
            {
                if (Handler != null)
                    Handler = Handler.MouseMove(arg);
                if (Handler == null)
                    return this.continuation(arg);
                return this;
            }

            public IMouseHandler MouseUp(MouseUpNotification arg)
            {
                if (Handler != null)
                    Handler = Handler.MouseUp(arg);
                if (Handler == null)
                    return this.continuation(arg);
                return this;
            }

            public IMouseHandler MouseClick(MouseClickNotification arg)
            {
                if (Handler != null)
                    Handler = Handler.MouseClick(arg);
                if (Handler == null)
                    return this.continuation(arg);
                return this;
            }

            public IMouseHandler MouseWheel(MouseWheelNotification arg)
            {
                if (Handler != null)
                    Handler = Handler.MouseWheel(arg);
                if (Handler == null)
                    return this.continuation(arg);
                return this;
            }

            public void Dispose()
            {
                Handler?.Dispose();
            }           
        }

        internal class ParallelMouseHandler : IMouseHandler
        {
            public IMouseHandler Left;
            public IMouseHandler Right;

            public ParallelMouseHandler(IMouseHandler left, IMouseHandler right)
            {
                Left = left;
                Right = right;
            }

            public void Dispose()
            {
                Left?.Dispose();
                Right?.Dispose();
            }

            public IMouseHandler MouseDown(MouseDownNotification arg)
            {
                if (Left != null) Left = Left.MouseDown(arg);
                if (Right != null) Right = Right.MouseDown(arg);
                return ThisIfLeftAndRightIsNotNull();
            }

            public IMouseHandler MouseMove(MouseMoveNotification arg)
            {
                if (Left != null) Left = Left.MouseMove(arg);
                if (Right != null) Right = Right.MouseMove(arg);
                return ThisIfLeftAndRightIsNotNull();
            }

            public IMouseHandler MouseUp(MouseUpNotification arg)
            {
                if (Left != null) Left = Left.MouseUp(arg);
                if (Right != null) Right = Right.MouseUp(arg);
                return ThisIfLeftAndRightIsNotNull();
            }

            public IMouseHandler MouseClick(MouseClickNotification arg)
            {
                if (Left != null) Left = Left.MouseClick(arg);
                if (Right != null) Right = Right.MouseClick(arg);
                return ThisIfLeftAndRightIsNotNull();
            }


            public IMouseHandler MouseWheel(MouseWheelNotification arg)
            {
                if (Left != null) Left = Left.MouseWheel(arg);
                if (Right != null) Right = Right.MouseWheel(arg);
                return ThisIfLeftAndRightIsNotNull();
            }

            protected virtual IMouseHandler ThisIfLeftAndRightIsNotNull()
            {
                if (Left == null)
                    return Right;
                if (Right == null)
                    return Left;
                return this;
            }

        }

        internal class SyncMouseHandler : ParallelMouseHandler
        {
            readonly IMouseHandler OrigLeft;
            readonly IMouseHandler OrigRight;

            public SyncMouseHandler(IMouseHandler left, IMouseHandler right)
                : base(left, right)
            {
                OrigLeft = left;
                OrigRight = right;
            }

            protected override IMouseHandler ThisIfLeftAndRightIsNotNull()
            {
                if (Left != OrigLeft || Right == null)
                    return Left;
                if (Right != OrigRight || Left == null)
                    return Right;
                return this;
            }
        }

        class WaitForEventMouseHandler : IMouseHandler
        {
            readonly Func<IMouseHandler, MouseDownNotification, IMouseHandler> onMouseDown;
            readonly Func<IMouseHandler, MouseMoveNotification, IMouseHandler> onMouseMove;
            readonly Func<IMouseHandler, MouseUpNotification, IMouseHandler> onMouseUp;
            readonly Func<IMouseHandler, MouseClickNotification, IMouseHandler> onMouseClick;
            readonly Func<IMouseHandler, MouseWheelNotification, IMouseHandler> onMouseWheel;

            public WaitForEventMouseHandler(
                Func<IMouseHandler, MouseDownNotification, IMouseHandler> onMouseDown,
                Func<IMouseHandler, MouseMoveNotification, IMouseHandler> onMouseMove,
                Func<IMouseHandler, MouseUpNotification, IMouseHandler> onMouseUp,
                Func<IMouseHandler, MouseClickNotification, IMouseHandler> onMouseClick,
                Func<IMouseHandler, MouseWheelNotification, IMouseHandler> onMouseWheel)
            {
                this.onMouseDown = onMouseDown;
                this.onMouseMove = onMouseMove;
                this.onMouseUp = onMouseUp;
                this.onMouseClick = onMouseClick;
                this.onMouseWheel = onMouseWheel;
            }

            public void Dispose() { }

            public IMouseHandler MouseDown(MouseDownNotification arg)
            {
                if (this.onMouseDown != null)
                    return this.onMouseDown(this, arg);
                return this;
            }

            public IMouseHandler MouseMove(MouseMoveNotification arg)
            {
                if (this.onMouseMove != null)
                    return this.onMouseMove(this, arg);
                return this;
            }

            public IMouseHandler MouseUp(MouseUpNotification arg)
            {
                if (this.onMouseUp != null)
                    return this.onMouseUp(this, arg);
                return this;
            }

            public IMouseHandler MouseClick(MouseClickNotification arg)
            {
                if (this.onMouseClick != null)
                    return this.onMouseClick(this, arg);
                return this;
            }

            public IMouseHandler MouseWheel(MouseWheelNotification arg)
            {
                if (this.onMouseWheel != null)
                    return this.onMouseWheel(this, arg);
                return this;
            }
        }

        class WhileEventMouseHandler : IMouseHandler
        {
            readonly Func<IMouseHandler, MouseDownNotification, IMouseHandler> onMouseDown;
            readonly Func<IMouseHandler, MouseMoveNotification, IMouseHandler> onMouseMove;
            readonly Func<IMouseHandler, MouseUpNotification, IMouseHandler> onMouseUp;
            readonly Func<IMouseHandler, MouseClickNotification, IMouseHandler> onMouseClick;
            readonly Func<IMouseHandler, MouseWheelNotification, IMouseHandler> onMouseWheel;


            public WhileEventMouseHandler(
                Func<IMouseHandler, MouseDownNotification, IMouseHandler> onMouseDown,
                Func<IMouseHandler, MouseMoveNotification, IMouseHandler> onMouseMove,
                Func<IMouseHandler, MouseUpNotification, IMouseHandler> onMouseUp,
                Func<IMouseHandler, MouseClickNotification, IMouseHandler> onMouseClick,
                Func<IMouseHandler, MouseWheelNotification, IMouseHandler> onMouseWheel)
            {
                this.onMouseDown = onMouseDown;
                this.onMouseMove = onMouseMove;
                this.onMouseUp = onMouseUp;
                this.onMouseClick = onMouseClick;
                this.onMouseWheel = onMouseWheel;

            }

            public void Dispose() { }

            public IMouseHandler MouseDown(MouseDownNotification arg)
            {
                if (this.onMouseDown != null)
                    return this.onMouseDown(this, arg);
                return null;
            }

            public IMouseHandler MouseMove(MouseMoveNotification arg)
            {
                if (this.onMouseMove != null)
                    return this.onMouseMove(this, arg);
                return null;
            }

            public IMouseHandler MouseUp(MouseUpNotification arg)
            {
                if (this.onMouseUp != null)
                    return this.onMouseUp(this, arg);
                return null;
            }

            public IMouseHandler MouseClick(MouseClickNotification arg)
            {
                if (this.onMouseClick != null)
                    return this.onMouseClick(this, arg);
                return null;
            }

            public IMouseHandler MouseWheel(MouseWheelNotification arg)
            {
                if (this.onMouseWheel != null)
                    return this.onMouseWheel(this, arg);
                return null;
            }
        }

        public static IMouseHandler DragMouseHandler(
                Action<MouseDownNotification> onDragStart,
                Action<MouseMoveNotification, Vector2> onDrag,
                Action<MouseNotification> onDragEnd)
        {
            return OnDragStart(
                down =>
                {
                    onDragStart?.Invoke(down);
                    return Drag(onDrag);
                }
            ).EndWith(onDragEnd);
        }

        public static IMouseHandler SelectionRectMouseHandler(
                Action<MouseDownNotification> onMouseDown,
                Action<MouseDownNotification> onSelectionStart,
                Action<MouseMoveNotification, RectangleF> onSelection,
                Action<MouseNotification> onSelectionFinish)
        {
            return OnDown(
                down =>
                {
                    onMouseDown(down);
                    return null;
                })              
                .InParallelWith(OnDragStart(
                        start =>
                        {
                            if (start.Buttons.HasFlag(MouseButtons.Left))
                            {
                                onSelectionStart?.Invoke(start);
                                return Select(start, onSelection).EndWith(onSelectionFinish); 
                            }
                            else
                            {
                                return null;
                            }
                        }
                    ));
        }

        /// <summary>
        /// Calls the given action once the handler is finished.
        /// </summary>
        public static IMouseHandler EndWith(this IMouseHandler handler, Action<MouseNotification> action)
        {
            return handler.ContinueWith(arg => { action(arg); return null; });
        }

        /// <summary>
        /// Creates a handler which will return the second handler once the first one is finished.
        /// </summary>
        public static IMouseHandler ContinueWith(this IMouseHandler handler, IMouseHandler nextHandler)
        {
            return handler.ContinueWith(_ => nextHandler);
        }

        /// <summary>
        /// Creates a handler which will call and return the handler of the given continuation once the first handler is finished.
        /// </summary>
        public static IMouseHandler ContinueWith(this IMouseHandler handler, Func<MouseNotification, IMouseHandler> continuation)
        {
            return new ContinuationMouseHandler(handler, continuation);
        }

        /// <summary>
        /// Creates a handler which runs both given handlers in parallel.
        /// As soon as one of them is finished the other one will be returned.
        /// </summary>
        public static IMouseHandler InParallelWith(this IMouseHandler left, IMouseHandler right)
        {
            return new ParallelMouseHandler(left, right);
        }

        /// <summary>
        /// Creates a handler which runs both given handlers in parallel.
        /// As soon as one of them changes, the changed one will be returned. left has priority of both change on the same event.
        /// Is any of the handlers finishes, the other is returned.
        /// </summary>
        public static IMouseHandler InSyncWith(this IMouseHandler left, IMouseHandler right)
        {
            return new SyncMouseHandler(left, right);
        }

        /// <summary>
        /// Creates a handler which will call the given continuation once a mouse down was observed.
        /// </summary>
        public static IMouseHandler OnDown(Func<MouseDownNotification, IMouseHandler> onDown)
        {
            return new WaitForEventMouseHandler((c, a) => onDown(a), null, null, null, null);
        }

        /// <summary>
        /// Creates a handler which will call the given continuation once a mouse move was observed.
        /// </summary>
        public static IMouseHandler OnMove(Func<MouseMoveNotification, IMouseHandler> onMove)
        {
            return new WaitForEventMouseHandler(null, (c, a) => onMove(a), null, null, null);
        }

        /// <summary>
        /// Creates a handler which will call the given continuation once a mouse up was observed.
        /// </summary>
        public static IMouseHandler OnUp(Func<MouseUpNotification, IMouseHandler> onUp)
        {
            return new WaitForEventMouseHandler(null, null, (c, a) => onUp(a), null, null);
        }

        /// <summary>
        /// Create a handler which will call the given continuation as long as any mouse actions are observed.
        /// In order to stay in the loop the continuation needs to return the given handler.
        /// In order to exit the loop the continuation needs to return null.
        /// </summary>
        public static IMouseHandler WhileAny(Func<IMouseHandler, MouseNotification, IMouseHandler> whileAny)
        {
            return new WhileEventMouseHandler(whileAny, whileAny, whileAny, whileAny, whileAny);
        }

        /// <summary>
        /// Create a handler which will call the given continuation as long as mouse moves are observed.
        /// In order to stay in the move loop the continuation needs to return the given handler.
        /// In order to exit the move loop the continuation needs to return null.
        /// </summary>
        public static IMouseHandler WhileMove(Func<IMouseHandler, MouseMoveNotification, IMouseHandler> whileMove)
        {
            return new WhileEventMouseHandler(null, whileMove, null, null, null);
        }

        /// <summary>
        /// Creates a handler which will call the given continuation once a mouse click was observed.
        /// </summary>
        public static IMouseHandler OnClick(Func<MouseClickNotification, IMouseHandler> onClick)
        {
            return new WaitForEventMouseHandler(null, null, null, (c, a) => onClick(a), null);
        }

        /// <summary>
        /// Creates a handler which will call the given continuation once a mouse click was observed.
        /// </summary>
        public static IMouseHandler OnWheel(Func<MouseWheelNotification, IMouseHandler> onWheel)
        {
            return new WaitForEventMouseHandler(null, null, null, null, (c, a) => onWheel(a));
        }

        /// <summary>
        /// Creates a handler which will call the given continuation once a mouse double click was observed.
        /// </summary>
        public static IMouseHandler OnDoubleClick(Func<MouseClickNotification, IMouseHandler> onDoubleClick)
        {
            return Handler.OnClick(click =>
            {
                if (click.ClickCount == 2)
                    return onDoubleClick(click);
                return null;
            });
        }

        /// <summary>
        /// Creates a handler which will call the given continuation once the start of a mouse drag was observed.
        /// </summary>
        public static IMouseHandler OnDragStart(Func<MouseDownNotification, IMouseHandler> onDragStart)
        {
            return Handler.OnDown(down =>
            {
                var clickArea = down.Position.GetRect(DoubleClickSize);
                return Handler.WhileMove((c, move) =>
                {
                    if (!clickArea.Contains(move.Position))
                        // Provide the client with the original down event
                        return onDragStart(down);
                    return c;
                });
            });
        }

        /// <summary>
        /// Creates a handler which will call the given action as long as the mouse moves. 
        /// Use together with OnDragStart and EndWith in order to setup proper initialization and finalization code.
        /// </summary>
        public static IMouseHandler Drag(Action<MouseMoveNotification, Vector2> drag)
        {
            return Handler.OnMove(start =>
            {
                var last = start;
                return Handler.WhileMove((c, move) =>
                {
                    var delta = move.Position - last.Position;
                    last = move;
                    drag(move, delta);
                    return c;
                });
            });
        }

        /// <summary>
        /// Creates a handler which will call the given action as long as the mouse moves.
        /// Use together with OnDragStart and EndWith in order to setup proper initialization and finalization code.
        /// </summary>
        public static IMouseHandler Select(MouseDownNotification initial, Action<MouseMoveNotification, RectangleF> selector)
        {
            return Handler.WhileMove((c, move) =>
            {
                var marquee = GetMarquee(move.Position, initial.Position);
                selector(move, marquee);
                return c;
            });
        }

        private static RectangleF GetMarquee(Vector2 currentLocation, Vector2 initialLocation)
        {
            return RectangleExtensions.FromLTRB(
                Math.Min(currentLocation.X, initialLocation.X),
                Math.Min(currentLocation.Y, initialLocation.Y),
                Math.Max(currentLocation.X, initialLocation.X),
                Math.Max(currentLocation.Y, initialLocation.Y));
        }
    }
}
