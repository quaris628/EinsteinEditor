using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.graphics.drawables;
using phi.control;
using phi.io;
using phi.phisics.Shapes;

namespace Einstein
{
    class Scene2 : Scene
    {
        private const string TITLE = "This is scene 2";

        private const string BACK_MSG = "Press Esc to go back";
        private const int BACK_MSG_Y = 20;
        private const Keys BACK_KEY = Keys.Escape;

        private const string SWITCH_MSG = "Press 3 to switch to scene 3";
        private const int SWITCH_MSG_Y = 40;
        private const Keys SWITCH_TO_3_KEY = Keys.D3;

        private const string COLL_MSG = "COLLISION DETECTED";
        private const string NOT_COLL_MSG = "COLLISION NOT DETECTED";
        private const int COLL_MSG_Y = 60;
        private bool coll = false;

        private const string INTERSECT_MSG = "INTERSECTION DETECTED";
        private const string NOT_INTERSECT_MSG = "INTERSECTION NOT DETECTED";
        private const int INTERSECT_MSG_Y = 80;

        private Text sceneTitle;
        private Text backMessage;
        private Text sceneSwitchMessage;
        private Text Collision_Detected;
        private Text Intersect_Detected;

        private Box box1 = new Box(0, 80, 32, 32);
        private Box box2 = new Box(40, 120, 32, 32);
        private Box box3 = new Box(100, 120, 32, 32);

        private Line l1;
        private Line l2;
        private Edge e1;
        private Edge e2;


        public Scene2(Scene prevScene) : base(prevScene)
        {
            sceneTitle = new Text.TextBuilder(TITLE).Build();
            backMessage = new Text.TextBuilder(BACK_MSG).WithY(BACK_MSG_Y).Build();
            sceneSwitchMessage = new Text.TextBuilder(SWITCH_MSG).WithY(SWITCH_MSG_Y).Build();
            Collision_Detected = new Text.TextBuilder(NOT_COLL_MSG).WithY(COLL_MSG_Y).Build();
            Intersect_Detected = new Text.TextBuilder("").WithY(INTERSECT_MSG_Y).Build();
        }

        protected override void InitializeMe()
        {
            IO.KEYS.Subscribe(Back, BACK_KEY);
            IO.FRAME_TIMER.Subscribe(run);
            IO.KEYS.Subscribe(moveBoxLeft, Keys.Left);
            IO.KEYS.Subscribe(moveBoxRight, Keys.Right);
            IO.KEYS.Subscribe(moveBoxUp, Keys.Up);
            IO.KEYS.Subscribe(moveBoxDown, Keys.Down);

            IO.RENDERER.Add(sceneTitle);
            IO.RENDERER.Add(backMessage);
            IO.RENDERER.Add(sceneSwitchMessage);
            IO.RENDERER.Add(box1.GetDrawable(), 1);
            IO.RENDERER.Add(box2.GetDrawable(), 1);
            IO.RENDERER.Add(box3.GetDrawable(), 1);
            IO.RENDERER.Add(Collision_Detected);
            IO.RENDERER.Add(Intersect_Detected);

            point p1 = new point(200, 180);
            point p2 = new point(100, 200);

            point p3 = new point(300, 180);
            point p4 = new point(320, 200);
            e1 = new Edge(p1, p2);
            e2 = new Edge(p3, p4);
            l1 = new Line(e1);
            l2 = new Line(e2);
            IO.RENDERER.Add(l1);
            IO.RENDERER.Add(l2);


        }

        public void run()
        {
            coll = box1.isColliding(box2) || box1.isColliding(box3);
            if (coll)
            {
                Collision_Detected.SetMessage(COLL_MSG);
            }
            else
            {
                Collision_Detected.SetMessage(NOT_COLL_MSG);
            }
            Intersect_Detected.SetMessage("Lines intersect? " + e1.Intersects(e2));
        }

        public void moveBoxLeft()
        {
            if (!box1.willCollide(box2, -5, 0))
                box1.updatePosition((int)box1.getX() - 5, (int)box1.getY());
            else
            {
                box1.updatePosition((int)box2.getX() + box2.GetWidth(), (int)box1.getY());
            }
            l1.SetX(l1.GetX() - 5);
            e1.shiftEdge(new point(-5, 0));
        }
        public void moveBoxRight()
        {
            if (!box1.willCollide(box2, 5, 0))
                box1.updatePosition((int)box1.getX() + 5, (int)box1.getY());
            else
            {
                box1.updatePosition((int)box2.getX() - box1.GetWidth(), (int)box1.getY());
            }
            l1.SetX(l1.GetX() + 5);
            e1.shiftEdge(new point(5, 0));
        }
        public void moveBoxUp()
        {
            if (!box1.willCollide(box2, 0, -5))
                box1.updatePosition((int)box1.getX(), (int)box1.getY() - 5);
            else
            {
                box1.updatePosition((int)box1.getX(), (int)box2.getY() + box2.GetHeight());
            }
            l1.SetY(l1.GetY() - 5);
            e1.shiftEdge(new point(0, -5));
        }
        public void moveBoxDown()
        {
            if (!box1.willCollide(box2, 0, 5))
                box1.updatePosition((int)box1.getX(), (int)box1.getY() + 5);
            else
            {
                box1.updatePosition((int)box1.getX(), (int)box2.getY() - box1.GetHeight());
            }
            l1.SetY(l1.GetY() + 5);
            e1.shiftEdge(new point(0, 5));
        }
    }
}
