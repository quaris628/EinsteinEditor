using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.graphics;
using phi.control;
using phi.io;
using phi.graphics.renderables;
using phi.graphics.drawables;

namespace Einstein
{
    class EditorScene : Scene
    {
        private const string TITLE = "Einstein Bibite Editor";

        private struct BACK_MSG
        {
            private const string MSG = "Press Escape to go back";
            private const int X = 0;
            private const int Y = 20;
            public static Text GetText()
            {
            return new Text.TextBuilder(MSG).WithXY(X, Y).Build();
            }
        }
        private const Keys BACK_KEY = Keys.Escape;

        private struct SWITCH_MSG
        {
            private const string MSG = "Press 2 to switch to scene 2";
            private const int X = 0;
            private const int Y = 40;
            public static Text GetText()
            {
            return new Text.TextBuilder(MSG).WithXY(X, Y).Build();
            }
        }
        private const Keys SWITCH_TO_2_KEY = Keys.D2;

        private struct BALL_TOGGLE
        {
            private const string IMAGE = EinsteinPhiConfig.RES_DIR + "ButtonBackground.png";
            private const string TEXT = "Bounce Ball";
            private const int X = 250;
            private const int Y = 10;
            // OnClick is BounceBall; see constructor, BounceBall is non-static
            public static Button GetButton(Action onClick)
            {
            return new Button.ButtonBuilder(new ImageWrapper(IMAGE), X, Y)
                .withText(TEXT).withOnClick(onClick).Build();
            }
        }

        private struct INPUT_NEURONS_BUTTON
        {
            private const string IMAGE = EinsteinPhiConfig.RES_DIR + "ButtonBackground.png";
            private const string TEXT = "Input Neurons";
            private const int X = 4;
            private const int Y = 4;
            public static Button GetButton(Action onClick)
            {
                return new Button.ButtonBuilder(new ImageWrapper(IMAGE), X, Y)
                    .withText(TEXT).withOnClick(onClick).Build();
            }
        }

        private struct OUTPUT_NEURONS_BUTTON
        {
            private const string IMAGE = EinsteinPhiConfig.RES_DIR + "ButtonBackground.png";
            private const string TEXT = "Output Neurons";
            private const int X = 4;
            private const int Y = 40;
            public static Button GetButton(Action onClick)
            {
                return new Button.ButtonBuilder(new ImageWrapper(IMAGE), X, Y)
                    .withText(TEXT).withOnClick(onClick).Build();
            }
        }

        private struct ADD_HIDDEN_NEURON_BUTTON
        {
            private const string IMAGE = EinsteinPhiConfig.RES_DIR + "ButtonBackground.png";
            private const string TEXT = "Add Hidden Neuron";
            private const int X = 4;
            private const int Y = 76;
            public static Button GetButton(Action onClick)
            {
                return new Button.ButtonBuilder(new ImageWrapper(IMAGE), X, Y)
                    .withText(TEXT).withOnClick(onClick).Build();
            }
        }


        private Button showInputNeurons;
        private Button showOutputNeurons;
        private Button addHiddenNeuron;
        private HashSet<Draggable> neurons;

        private Text sceneTitle;
        private Text backMessage;
        private Text sceneSwitchMessage;
        private Ball ball;
        private bool ballToggler;
        private Button ballToggle;
        private Draggable dragger;
        private ProgressBar bar;
        private ProgressCircle circle;
        private ProgressCircle clock;

        public EditorScene(Scene prevScene) : base(prevScene, new ImageWrapper(EinsteinPhiConfig.Render.DEFAULT_BACKGROUND))
        {
            showInputNeurons = INPUT_NEURONS_BUTTON.GetButton(showInputNeuronsButtonOnClick);
            showOutputNeurons = OUTPUT_NEURONS_BUTTON.GetButton(showOutputNeuronsButtonOnClick);
            addHiddenNeuron = ADD_HIDDEN_NEURON_BUTTON.GetButton(addHiddenNeuronButtonOnClick);

            /*
            sceneTitle = new Text.TextBuilder(TITLE).Build();
            backMessage = BACK_MSG.GetText();
            sceneSwitchMessage = SWITCH_MSG.GetText();
            ball = new Ball(0, 50);
            ballToggle = BALL_TOGGLE.GetButton(BounceBall);
            dragger = new Draggable(ballToggle);

            bar = new ProgressBar(10, 300, 100, 30, 100);
            circle = new ProgressCircle(300, 300, 50, 100);
            clock = new ProgressCircle(140, 300, 50, EinsteinPhiConfig.Render.FPS);
            //*/
        }

        protected override void InitializeMe()
        {
            IO.RENDERER.Add(showInputNeurons);
            IO.RENDERER.Add(showOutputNeurons);
            IO.RENDERER.Add(addHiddenNeuron);
            showInputNeurons.Initialize();
            showOutputNeurons.Initialize();
            addHiddenNeuron.Initialize();

            /*
            IO.KEYS.Subscribe(Back, BACK_KEY);
            IO.KEYS.Subscribe(SwitchTo2, SWITCH_TO_2_KEY);
            IO.FRAME_TIMER.Subscribe(MoveBall);
            IO.FRAME_TIMER.Subscribe(clock_tick);
            IO.RENDERER.Add(sceneTitle);
            IO.RENDERER.Add(backMessage);
            IO.RENDERER.Add(sceneSwitchMessage);
            IO.RENDERER.Add(ball.GetDrawable(), 1);
            IO.RENDERER.Add(ballToggle);
            IO.RENDERER.Add(bar);
            IO.RENDERER.Add(circle);
            IO.RENDERER.Add(clock);
            IO.KEYS.Subscribe(AddProgress, Keys.Up);
            IO.KEYS.Subscribe(RemoveProgress, Keys.Down);
            ballToggle.Initialize();
            dragger.Initialize();
            //*/
        }

        public void showInputNeuronsButtonOnClick()
        {
            // TODO
        }

        public void showOutputNeuronsButtonOnClick()
        {
            // TODO
        }

        public void addHiddenNeuronButtonOnClick()
        {
            // TODO
        }

        /*
        public void SwitchTo2()
        {
            SwitchTo(new Scene2(this));
        }

        public void SwitchToSettings()
        {

        }
        public void AddProgress()
        {
            bar.AddProgress(5);
            circle.AddProgress(5);
        }
        public void RemoveProgress()
        {
            bar.RemoveProgress(5);
            circle.RemoveProgress(5);
        }

        public void clock_tick()
        {
            if(clock.getCurrentProgress() <= 0)
            {
            clock.SetToMax();
            }
            clock.RemoveProgress();
            
        }
        public void MoveBall()
        {
            if(ballToggler)
            {
            if(ball.GetDrawable().GetX() > 600 - ball.GetDrawable().GetWidth())
            {
                ballToggler = !ballToggler;
            }
            else
            {
                ball.GetDrawable().SetX(ball.GetDrawable().GetX() + 3);
            }
            }
            else
            {
            if (ball.GetDrawable().GetX() <= 0)
            {
                ballToggler = !ballToggler;
            }
            else
            {
                ball.GetDrawable().SetX(ball.GetDrawable().GetX() - 3);
            }
            }
        }

        public void BounceBall()
        {
            ballToggler = !ballToggler;
        }
        //*/
    }
}
