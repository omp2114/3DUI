/************************************************************************************ 
 * Copyright (c) 2008-2012, Columbia University
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Columbia University nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY COLUMBIA UNIVERSITY ''AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * 
 * ===================================================================================
 * Author: Ohan Oda (ohan@cs.columbia.edu)
 * 
 *************************************************************************************/

// Uncomment this line if you want to use the pattern-based marker tracking
//#define USE_PATTERN_MARKER

using System;
using System.Collections.Generic;
using System.Windows.Media;
using GoblinXNA.Graphics.ParticleEffects2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Helpers;
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;

namespace Tutorial8___Optical_Marker_Tracking___PhoneLib
{
    public class Tutorial8_Phone
    {
        SpriteFont sampleFont;
        Scene scene;
        
        // Daniel
        Texture2D helpTexture1,
            helpTexture2,
            helpTexture3,
            helpTexture4,
            helpTexture5,
            helpTexture6,
            helpTexture7,
            helpTexture8,
            helpTexture9,
            helpTexture10,
            helpTexture11,
            helpTexture12;

        G2DPanel helpPanel;
        G2DButton helpScreen;
        ContentManager GlobalContent;
        private int pageNumber;

        //Pat
        Dictionary<int,Matrix> fire_matrices  = new Dictionary<int, Matrix>();
        List<Vector3> fire_positions = new List<Vector3>();
        private MiniMapNavigation navigator;
        private TransformNode firesRepTransNode;

        private GeometryNode vrCameraRepNode;
        private TransformNode vrCameraRepTransNode;

        private CameraNode arCameraNode, vrCameraNode;

        private RenderTarget2D arViewRenderTarget;
        private RenderTarget2D vrViewRenderTarget;
        private Rectangle arViewRect;
        private Rectangle vrViewRect;

        private Texture2D videoTexture;

        TransformNode boxTransNode;
        TransformNode fireTransNode;
        TransformNode fireTransNode2;
        TransformNode fireTransNode3;
        TransformNode noTransform;
        TransformNode yesTransform;
        TransformNode helpTransform;
        TransformNode exitTransform;
        ////boxTransNode.AddChild(hoseTransNode);
        ////boxTransNode.AddChild(boxNode);
        TransformNode hoseTransNode;
        TransformNode plusTransNode;
        TransformNode minusTransNode;

        private ParticleNode fireEffectNode;
        private ParticleNode fireEffectNode2;
        private ParticleNode fireEffectNode3;

        private ParticleNode smokeEffectNode;
        private ParticleNode smokeEffectNode2;
        private ParticleNode smokeEffectNode3;


        private bool isDrawingMiniMap = false;

        private bool have_registered_fires = false;

        // Seungwoo
        MarkerNode waterMarkerNode, fireMarkerNode1, fireMarkerNode2, fireMarkerNode3;
        bool useStaticImage = false;
        bool useSingleMarker = false;
        bool betterFPS = true; // has trade-off of worse tracking if set to true

        Viewport viewport;//

        Random random = new Random();

        private FireParticleEffect fireParticles1, fireParticles2, fireParticles3;
        private ExplosionParticleEffect explosionParticles;
        private SmokePlumeParticleEffect smokeParticles1, smokeParticles2, smokeParticles3;
        private WaterParticleEffect waterParticles;

        private int extinguished1 = 0, extinguished2 = 0, extinguished3 = 0;

        private Matrix a, b, c;
        
        // Orlando
        private float countdown = 5;
        private float timer = 0;
        // Different states for the game state manager.
        string SCOPESTATE = "scope";
        string ACTIVESTATE = "game";
        string IDLESTATE = "idle";
        bool HELPSTATE = false;
        bool MANYPAGES = false;
        private string EXITGAMECONFIRMATION = "Are you sure \n    you want \n    to exit?";
        private string INGAMEHELPSTATE = "ingamehelp";
        private string IDLEHELPSTATE = "idlehelpstate";
        string GAMESTATE;
        private string PREVIOUS;
        private string CONFIRMATION = "CONFIRMATION";
        private string COUNTDOWNSTATE = "COUNTDOWN";
        private string MENUSTATE;
        private string MENUSTATEON = "menustateon";
        private string MENUSTATEOFF = "menustateoff";
        private float waterPressure;
        private GeometryNode exitNode, helpNode, yesNode, noNode;
        private Model exit, help, start, yes, no, next, prev;
        private MarkerNode groundMarkerNode,
                           nozzlePointerNode,
                           valveNode,
                           startMarkerNode,
                           helpMarkerNode, yesMarkerNode, noMarkerNode;

#if USE_PATTERN_MARKER
        float markerSize = 32.4f;
#else
        float markerSize = 32.4f;
#endif

        public Tutorial8_Phone()
        {
            // no contents
        }

        public Texture2D VideoBackground
        {
            get { return videoTexture; }
            set { videoTexture = value; }
        }

        public void Initialize(IGraphicsDeviceService service, ContentManager content, VideoBrush videoBrush)
        {
            // Daniel
            GlobalContent = content;

            // Orlando
            MENUSTATE = MENUSTATEOFF;
            GAMESTATE = IDLESTATE;

            // Seungwoo
            viewport = new Viewport(80, 0, 640, 480);
            viewport.MaxDepth = service.GraphicsDevice.Viewport.MaxDepth;
            viewport.MinDepth = service.GraphicsDevice.Viewport.MinDepth;
            service.GraphicsDevice.Viewport = viewport;

            // Initialize the GoblinXNA framework
            State.InitGoblin(service, content, "");

            LoadContent(content);
            

            // Initialize the scene graph
            scene = new Scene();

            // Set up the lights used in the scene
            CreateLights();

            CreateCamera();

            SetupViewPort();

            SetupMarkerTracking(videoBrush);

            CreateVirtualCameraRepresentation();

            CreateObjects();

            State.ShowNotifications = true;
            Notifier.Font = sampleFont;

            State.ShowFPS = true;


        }

        private void CreateVirtualCameraRepresentation()
        {
            vrCameraRepNode = new GeometryNode("VR Camera")
            {
                Model = new Pyramid(markerSize * 4/3, markerSize, markerSize),
                Material =
                {
                    Diffuse = Color.Orange.ToVector4(),
                    Specular = Color.White.ToVector4(),
                    SpecularPower = 20
                }
            };

            vrCameraRepTransNode = new TransformNode()
                {
                    Translation = new Vector3(0,0,200)
                };

            TransformNode camOffset = new TransformNode()
            {
                Translation = new Vector3(0,0,0),
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -90)
            };

            scene.RootNode.AddChild(vrCameraRepTransNode);
            vrCameraRepTransNode.AddChild(camOffset);
            camOffset.AddChild(vrCameraRepNode);
        }

        private void SetupViewPort()
        {
            PresentationParameters pp = State.Device.PresentationParameters;

            // Create a render target to render the AR scene to
            arViewRenderTarget = new RenderTarget2D(State.Device, viewport.Width, viewport.Height, false,
                SurfaceFormat.Color, pp.DepthStencilFormat);

            // Create a render target to render the VR scene to.
            vrViewRenderTarget = new RenderTarget2D(State.Device, viewport.Width * 4 / 15, viewport.Height * 4 / 15, false,
                SurfaceFormat.Color, pp.DepthStencilFormat);

            // Set the AR scene to take the full window size
            arViewRect = new Rectangle(0, 0, viewport.Width, viewport.Height);

            // Set the VR scene to take the 2 / 5 of the window size and positioned at the top right corner
            vrViewRect = new Rectangle(viewport.Width - vrViewRenderTarget.Width, 0,
                vrViewRenderTarget.Width, vrViewRenderTarget.Height);
        }

        private void CreateCamera()
        {
            // Create a camera for VR scene 
            Camera vrCamera = new Camera();
            vrCamera.Translation = new Vector3(0, 480, 0);
            vrCamera.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(-90));
            vrCamera.FieldOfViewY = MathHelper.ToRadians(60);
            vrCamera.ZNearPlane = 1;
            vrCamera.ZFarPlane = 2000;

            vrCameraNode = new CameraNode(vrCamera);
            scene.RootNode.AddChild(vrCameraNode);

            // Create a camera for AR scene
            Camera arCamera = new Camera();
            arCamera.ZNearPlane = 1;
            arCamera.ZFarPlane = 2000;

            arCameraNode = new CameraNode(arCamera);
            scene.RootNode.AddChild(arCameraNode);

            // Set the AR camera to be the main camera so that at the time of setting up the marker tracker,
            // the marker tracker will assign the right projection matrix to this camera
            scene.CameraNode = arCameraNode;
            
        }

        private void CreateLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(1, -1, -1);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.AmbientLightColor = new Vector4(0.2f, 0.2f, 0.2f, 1);
            lightNode.LightSource = lightSource;

            scene.RootNode.AddChild(lightNode);
        }

        private void SetupMarkerTracking(VideoBrush videoBrush)
        {
            IVideoCapture captureDevice = null;

            if (useStaticImage)
            {
                captureDevice = new NullCapture();
                captureDevice.InitVideoCapture(0, FrameRate._30Hz, Resolution._320x240,
                    ImageFormat.B8G8R8A8_32, false);
                if(useSingleMarker)
                    ((NullCapture)captureDevice).StaticImageFile = "MarkerImageHiro.jpg";
                else
                    ((NullCapture)captureDevice).StaticImageFile = "MarkerImage_320x240";

                scene.ShowCameraImage = true;
            }
            else
            {
                captureDevice = new PhoneCameraCapture(videoBrush);
                captureDevice.InitVideoCapture(0, FrameRate._30Hz, Resolution._640x480,
                    ImageFormat.B8G8R8A8_32, false);
                ((PhoneCameraCapture)captureDevice).UseLuminance = true;

                if (betterFPS)
                    captureDevice.MarkerTrackingImageResizer = new HalfResizer();
            }

            // Add this video capture device to the scene so that it can be used for
            // the marker tracker
            scene.AddVideoCaptureDevice(captureDevice);

#if USE_PATTERN_MARKER
            NyARToolkitTracker tracker = new NyARToolkitTracker();
#else
            NyARToolkitIdTracker tracker = new NyARToolkitIdTracker();
#endif

            if (captureDevice.MarkerTrackingImageResizer != null)
                tracker.InitTracker((int)(captureDevice.Width * captureDevice.MarkerTrackingImageResizer.ScalingFactor),
                    (int)(captureDevice.Height * captureDevice.MarkerTrackingImageResizer.ScalingFactor),
                    "camera_para.dat");
            else
                tracker.InitTracker(captureDevice.Width, captureDevice.Height, "camera_para.dat");

            // Set the marker tracker to use for our scene
            scene.MarkerTracker = tracker;
        }

        private void CreateObjects()
        {
            // Create a marker node to track a ground marker array.
#if USE_PATTERN_MARKER
            if(useSingleMarker)
                groundMarkerNode = new MarkerNode(scene.MarkerTracker, "patt.hiro", 16, 16, markerSize, 0.7f);
            else
                groundMarkerNode = new MarkerNode(scene.MarkerTracker, "NyARToolkitGroundArray.xml", 
                    NyARToolkitTracker.ComputationMethod.Average);
#else

            // Daniel
            helpPanel = new G2DPanel();
            helpScreen = new G2DButton();
            helpScreen.Bounds = new Rectangle(0, 0, 640, 160);
            helpPanel.AddChild(helpScreen);
            pageNumber = 1;

            // Orlando
            waterMarkerNode = new MarkerNode(scene.MarkerTracker, "NyARToolkitIDToolbar1.xml",
                NyARToolkitTracker.ComputationMethod.Average);
            valveNode = new MarkerNode(scene.MarkerTracker, "NyARToolkitIDToolbar2.xml",
               NyARToolkitTracker.ComputationMethod.Average);
            startMarkerNode = new MarkerNode(scene.MarkerTracker, "StartMarker.xml",
               NyARToolkitTracker.ComputationMethod.Average);
            helpMarkerNode = new MarkerNode(scene.MarkerTracker, "HelpMarker.xml",
               NyARToolkitTracker.ComputationMethod.Average);
            yesMarkerNode = new MarkerNode(scene.MarkerTracker, "NoMarker.xml",
                NyARToolkitTracker.ComputationMethod.Average);
            noMarkerNode = new MarkerNode(scene.MarkerTracker, "YesMarker.xml",
               NyARToolkitTracker.ComputationMethod.Average);



            scene.RootNode.AddChild(waterMarkerNode);
            scene.RootNode.AddChild(valveNode);

            // UI Buttons
            scene.RootNode.AddChild(startMarkerNode);
            scene.RootNode.AddChild(helpMarkerNode);
            scene.RootNode.AddChild(yesMarkerNode);
            scene.RootNode.AddChild(noMarkerNode);

            // Load in the model loader
            ModelLoader loader = new ModelLoader();

            exit = (Model)loader.Load("", "exit");
            start = (Model)loader.Load("", "start");
            exitNode = new GeometryNode("exit");
            help = (Model)loader.Load("", "help");
            helpNode = new GeometryNode("help");
            yes = (Model)loader.Load("", "no");
            no = (Model)loader.Load("", "yes");
            next = (Model)loader.Load("", "next");
            prev = (Model)loader.Load("", "prev");
            yesNode = new GeometryNode("yes");
            noNode = new GeometryNode("no");

            noNode.Model = null;// no;
            yesNode.Model = null;// yes;
            exitNode.Model = null;// start;
            helpNode.Model = null;// help;

            // Create exit button material
            Material exitMaterial = new Material();
            exitMaterial.Diffuse = Color.Tomato.ToVector4();
            exitMaterial.Specular = Color.White.ToVector4();
            exitMaterial.SpecularPower = 10;
            exitNode.Material = exitMaterial;

            // Create help button material
            Material helpMaterial = new Material();
            helpMaterial.Diffuse = Color.Purple.ToVector4();
            helpMaterial.Specular = Color.White.ToVector4();
            helpMaterial.SpecularPower = 10;
            helpNode.Material = helpMaterial;

            // Create yes button material
            Material yesMaterial = new Material();
            yesMaterial.Diffuse = Color.Red.ToVector4();
            yesMaterial.Specular = Color.White.ToVector4();
            yesMaterial.SpecularPower = 10;
            yesNode.Material = yesMaterial;

            // Create no button material
            Material noMaterial = new Material();
            noMaterial.Diffuse = Color.Green.ToVector4();
            noMaterial.Specular = Color.White.ToVector4();
            noMaterial.SpecularPower = 10;
            noNode.Material = noMaterial;

            
            // Get the dimensions for the markers. They should be the same but i got 2 just in case.
            Vector3 dimension = Vector3Helper.GetDimensions(exit.MinimumBoundingBox);
            float scale = markerSize / Math.Max(dimension.X, dimension.Y);
            Vector3 dimension2 = Vector3Helper.GetDimensions(help.MinimumBoundingBox);
            float scale2 = markerSize / Math.Max(dimension2.X, dimension2.Y);
            // Create the transform node for exit
            exitTransform = new TransformNode()
            {
                Translation = new Vector3(50f, 60f, dimension.Y * scale / 2),
                Rotation =
                    Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(90)) *
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(90)),
                Scale = new Vector3(scale, scale, scale)
            };
            
            helpTransform = new TransformNode()
            {

                Translation = new Vector3(50f, 60f, dimension2.Y * scale2 / 2),
                Rotation =
                    Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(90)) *
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(90)),
                Scale = new Vector3(scale2, scale2, scale2)
            };
           
            yesTransform = new TransformNode()
            {
                Translation = new Vector3(-50f, 0, 0),
                Rotation =
                    Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(90)) *
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(90)),
                Scale = new Vector3(scale, scale, scale)
            };
            
            noTransform = new TransformNode()
            {
                Translation = new Vector3(50f, 0, 0),
                Rotation =
                    Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(90)) *
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(90)),
                Scale =new Vector3(scale2, scale2, scale2)
            };
           
            // Seungwoo
            fireMarkerNode1 = new MarkerNode(scene.MarkerTracker, "Fire2 - (2x1) Marker.xml",
                NyARToolkitTracker.ComputationMethod.Average);

            fireMarkerNode2 = new MarkerNode(scene.MarkerTracker, "Fire1 - (2x1) Marker.xml",
                NyARToolkitTracker.ComputationMethod.Average);

            fireMarkerNode3 = new MarkerNode(scene.MarkerTracker, "Fire3 - (2x1) Marker.xml",
                            NyARToolkitTracker.ComputationMethod.Average);
            
 
#endif
            Model hose = (Model) loader.Load("", "hose");
            hose.UseInternalMaterials = true;


            // Hose
            ModelLoader hoseloader = new ModelLoader();
            Model hoseModel = (Model)hoseloader.Load("", "hose");
            GeometryNode hoseNode = new GeometryNode("hose");
            hoseNode.Model = hoseModel;

            ((Model)hoseNode.Model).UseInternalMaterials = true;

            hoseTransNode = new TransformNode();

            hoseTransNode.Translation = new Vector3(0, -40, 0);
            hoseTransNode.Scale = new Vector3(40, 40, 40);
            

            // Plus Controller
            ModelLoader plusloader = new ModelLoader();
            Model plusModel = (Model)plusloader.Load("", "plus");
            GeometryNode plusNode = new GeometryNode("plus");
            plusNode.Model = plusModel;

            ((Model)plusNode.Model).UseInternalMaterials = true;

            plusTransNode = new TransformNode();

            plusTransNode.Translation = new Vector3(-20, -10, 0);
            plusTransNode.Scale = new Vector3(60, 60, 60);
            plusTransNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX,
                            MathHelper.ToRadians(90));

            // Minus Controller
            ModelLoader minusloader = new ModelLoader();
            Model minusModel = (Model)minusloader.Load("", "minus");
            GeometryNode minusNode = new GeometryNode("minus");
            minusNode.Model = minusModel;

            ((Model)minusNode.Model).UseInternalMaterials = true;

            minusTransNode = new TransformNode();

            minusTransNode.Translation = new Vector3(40, 20, 0);
            minusTransNode.Scale = new Vector3(60, 60, 60);
            minusTransNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX,
                            MathHelper.ToRadians(90));
            //minusTransNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY,
            //                MathHelper.ToRadians(90));




            //scene.RootNode.AddChild(waterMarkerNode);
            scene.RootNode.AddChild(fireMarkerNode1);
            scene.RootNode.AddChild(fireMarkerNode2);
            scene.RootNode.AddChild(fireMarkerNode3);

            // Create a geometry node with a model of a box that will be overlaid on
            // top of the ground marker array initially. (When the toolbar marker array is
            // detected, it will be overlaid on top of the toolbar marker array.)
            GeometryNode boxNode = new GeometryNode("Box");
            boxNode.Model = new Box(markerSize);

            // Create a material to apply to the box model
            Material boxMaterial = new Material();
            boxMaterial.Diffuse = new Vector4(0.5f, 0, 0, 1);
            boxMaterial.Specular = Color.White.ToVector4();
            boxMaterial.SpecularPower = 10;

            boxNode.Material = boxMaterial;

            boxTransNode = new TransformNode();
           
            fireTransNode = new TransformNode();

            fireTransNode2= new TransformNode();

            fireTransNode3 = new TransformNode();

            
            noTransform.AddChild(noNode);
            yesTransform.AddChild(yesNode);
            helpTransform.AddChild(helpNode);
            exitTransform.AddChild(exitNode);
            boxTransNode.AddChild(boxNode);

            
#if WINDOWS_PHONE
            waterParticles = new WaterParticleEffect(50, true);
            fireParticles1 = new FireParticleEffect(50);
            fireParticles2 = new FireParticleEffect(30);
            fireParticles3 = new FireParticleEffect(80);
            explosionParticles = new ExplosionParticleEffect(1);

            smokeParticles1 = new SmokePlumeParticleEffect(30);
            smokeParticles2 = new SmokePlumeParticleEffect(30);
            smokeParticles3 = new SmokePlumeParticleEffect(30);

#else
            SmokePlumeParticleEffect smokeParticles = new SmokePlumeParticleEffect();
            FireParticleEffect fireParticles = new FireParticleEffect();
            // The order defines which particle effect to render first. Since we want
            // to show the fire particles in front of the smoke particles, we make
            // the smoke particles to be rendered first, and then fire particles
            smokeParticles.DrawOrder = 200;
            fireParticles.DrawOrder = 300;
#endif
            // Create a particle node to hold these two particle effects
            ParticleNode WaterEffectNode = new ParticleNode();
            WaterEffectNode.ParticleEffects.Add(waterParticles);


            fireEffectNode = new ParticleNode();
            fireEffectNode.ParticleEffects.Add(fireParticles1);

            fireEffectNode.ParticleEffects.Add(explosionParticles);

            smokeEffectNode = new ParticleNode();
            smokeEffectNode.ParticleEffects.Add(smokeParticles1);
            smokeParticles1.Enabled = false;

            fireEffectNode2 = new ParticleNode();
            fireEffectNode2.ParticleEffects.Add(fireParticles2);

            smokeEffectNode2 = new ParticleNode();
            smokeEffectNode2.ParticleEffects.Add(smokeParticles2);
            smokeParticles2.Enabled = false;

            fireEffectNode3 = new ParticleNode();
            fireEffectNode3.ParticleEffects.Add(fireParticles3);

            smokeEffectNode3 = new ParticleNode();
            smokeEffectNode3.ParticleEffects.Add(smokeParticles3);
            smokeParticles3.Enabled = false;

            // Implement an update handler for each of the particle effects which will be called
            // every "Update" call 

            
            WaterEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
            
            fireEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
            smokeEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
            fireEffectNode2.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
            smokeEffectNode2.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
            fireEffectNode3.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
            smokeEffectNode3.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
      
            hoseTransNode.AddChild(hoseNode);
            plusTransNode.AddChild(plusNode);
            minusTransNode.AddChild(minusNode);

            fireTransNode.AddChild(fireEffectNode);
            fireTransNode.AddChild(smokeEffectNode);

            fireTransNode2.AddChild(fireEffectNode2);
            fireTransNode2.AddChild(smokeEffectNode2);

            fireTransNode3.AddChild(fireEffectNode3);
            fireTransNode3.AddChild(smokeEffectNode3);

      
            boxTransNode.AddChild(WaterEffectNode);
            scene.RootNode.AddChild(boxTransNode);
            fireMarkerNode1.AddChild(fireTransNode);
            fireMarkerNode2.AddChild(fireTransNode2);
            fireMarkerNode3.AddChild(fireTransNode3);

            noMarkerNode.AddChild(noTransform);
            yesMarkerNode.AddChild(yesTransform);
            helpMarkerNode.AddChild(helpTransform);
            startMarkerNode.AddChild(exitTransform);
            waterMarkerNode.AddChild(hoseTransNode);
            valveNode.AddChild(plusTransNode);
            valveNode.AddChild(minusTransNode);
        }

        // Function that displays the 3D menu!
        private void showUI()
        {
            //Notifier.AddMessage("RunningDBKDBKABDKBDKABDKABKADBK");
            // If we are currently in the gamestate, we want to provide the user 
            // the option to exit the game
            if (GAMESTATE.Equals(ACTIVESTATE) || GAMESTATE.Equals(COUNTDOWNSTATE))
            {
                exitNode.Model = exit;
                yesNode.Model = null;
                noNode.Model = null;
            }
            else if (GAMESTATE.Equals(IDLESTATE))
            {
                exitNode.Model = start;
                yesNode.Model = null;
                noNode.Model = null;
            }
            else
            {
                exitNode.Model = null;
            }
            // If we are currently in the helpstate, we want to provide the user the option to exit the game
            if (HELPSTATE || GAMESTATE.Equals(IDLEHELPSTATE))
            {
                helpNode.Model = exit;
                yesNode.Model = next;
                noNode.Model = prev;
                helpScreen.Texture = GlobalContent.Load<Texture2D>("Help" + pageNumber.ToString());
                scene.UIRenderer.Add2DComponent(helpPanel);
            }
            else
            {
                helpNode.Model = help;
                yesNode.Model = null;
                noNode.Model = null;
                scene.UIRenderer.Remove2DComponent(helpPanel);
            }
            if (GAMESTATE.Equals(CONFIRMATION))
            {
                yesNode.Model = yes;
                noNode.Model = no;
            }


        }
        /// <summary>
        /// Update the fire effect on the torus model
        /// </summary>
        /// <param name="worldTransform"></param>
        /// <param name="particleEffects"></param>
        private void UpdateRingOfFire(Matrix worldTransform, List<ParticleEffect> particleEffects)
        {
            //Notifier.AddMessage(fireMarkerNode1.WorldTransformation.Translation.ToString());
            // Update Fire 1 to revive it
            if (extinguished1 > 200 && extinguished1 < 700)
            {
                extinguished1++;
            }
            else if (extinguished1 == 700)
            {
                fireParticles1.Enabled = true;
            }

            foreach (ParticleEffect particle in particleEffects)
            {
                if (particle is FireParticleEffect && GAMESTATE.Equals(ACTIVESTATE))
                {
#if WINDOWS_PHONE
                    if (particle == fireParticles1 && fireMarkerNode1.MarkerFound)
                    {
                        particle.AddParticles(Project(fireMarkerNode1.WorldTransformation.Translation));
                    }
                    else if (particle == fireParticles2 && fireMarkerNode2.MarkerFound)
                    {
                        particle.AddParticles(Project(fireMarkerNode2.WorldTransformation.Translation));
                    }
                    else if (particle == fireParticles3 && fireMarkerNode3.MarkerFound)
                    {
                        particle.AddParticles(Project(fireMarkerNode3.WorldTransformation.Translation));
                    }
#else   
    // Add 10 fire particles every frame
                    for (int k = 0; k < 10; k++)
                        particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), Vector3.Zero);
#endif
                }
                else if (particle is WaterParticleEffect && waterMarkerNode.MarkerFound)
                {
#if WINDOWS_PHONE
                    Vector3 vel = new Vector3(0,10,0);//groundMarkerNode.WorldTransformation.Translation;                    
                    Quaternion rotation = new Quaternion();
                    Vector3 pos = new Vector3();
                    Vector3 scale = new Vector3();

                    waterMarkerNode.WorldTransformation.Decompose(out pos, out rotation, out scale);

                    pos = waterMarkerNode.WorldTransformation.Translation;
                    vel = Vector3.Transform(vel, Matrix.CreateFromQuaternion(rotation));
                    vel = 30 * vel;

                    Vector3 shiftedPos = new Vector3(0,0,0);
                    shiftedPos = Vector3.Transform(shiftedPos, waterMarkerNode.WorldTransformation);
                    if (valveNode.MarkerFound)
                        particle.MaxScale = waterPressure + 0.75f;
                    else
                    {
                    }
                    particle.AddParticles3D(shiftedPos, vel);
                    Vector3 temp = new Vector3();
                    temp = fireMarkerNode1.WorldTransformation.Translation;
                    temp.X = temp.X + markerSize;
                    temp.Y = temp.Y - markerSize;

                    Vector3 temp2 = new Vector3();
                    temp2 = fireMarkerNode2.WorldTransformation.Translation;
                    temp2.X = temp2.X + markerSize;
                    temp2.Y = temp2.Y - markerSize;

                    Vector3 temp3 = new Vector3();
                    temp3 = fireMarkerNode3.WorldTransformation.Translation;
                    temp3.X = temp3.X + markerSize;
                    temp3.Y = temp3.Y - markerSize;

                    if (((WaterParticleEffect)particle).isCollide(temp) && fireMarkerNode1.MarkerFound)
                    {                                                
                        if (extinguished1 == 740)
                        {
                            smokeParticles1.Enabled = true;
                            fireParticles1.Enabled = false;
                            explosionParticles.Enabled = false;
                            extinguished1++;
                        }
                        else if (extinguished1 == 150)
                        {
                            explosionParticles.Enabled = false;
                            //fireParticles.Enabled = false;
                            extinguished1++;
                        }
                        else if (extinguished1 == 200)
                        {
                            fireParticles1.Enabled = false;
                            extinguished1++;
                        }
                        else
                        {
                            extinguished1++;
                        }
                    }
                    else if (((WaterParticleEffect)particle).isCollide(temp2) && fireMarkerNode2.MarkerFound)
                    {

                        if (extinguished2 == 200)
                        {
                            fireParticles2.Enabled = false;
                            smokeParticles2.Enabled = true;
                            extinguished2++;
                        }
                        else if(extinguished2 < 200)
                            extinguished2++;
                    }
                    else if (((WaterParticleEffect)particle).isCollide(temp3) && fireMarkerNode3.MarkerFound)
                    {

                        if (extinguished3 == 200)
                        {
                            fireParticles3.Enabled = false;
                            smokeParticles3.Enabled = true;
                            extinguished3++;
                        }
                        else if (extinguished3 < 200)
                            extinguished3++;
                    }
                }
#else   
                    // Add 10 fire particles every frame
                    for (int k = 0; k < 10; k++)
                        particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), Vector3.Zero);
#endif

                else if (particle is SmokePlumeParticleEffect)
                {
#if WINDOWS_PHONE
                    if (particle == smokeParticles1 && extinguished1 > 740 && extinguished1 < 850)
                    {
                        for (int cnt = 0; cnt < 100; cnt++)
                        {
                            particle.AddParticles(Project(fireMarkerNode1.WorldTransformation.Translation));
                        }
                        extinguished1++;
                    }
                    else if (particle == smokeParticles2 && extinguished2 >= 201 && extinguished2 < 330)
                    {
                        for (int cnt = 0; cnt < 100; cnt++)
                        {
                            particle.AddParticles(Project(fireMarkerNode2.WorldTransformation.Translation));
                        }
                        extinguished2++;
                    }
                    else if (particle == smokeParticles3 && extinguished3 >= 201 && extinguished3 < 330)
                    {
                        for (int cnt = 0; cnt < 100; cnt++)
                        {
                            particle.AddParticles(Project(fireMarkerNode3.WorldTransformation.Translation));
                        }
                        extinguished3++;
                    }               
#else
    // Add 1 smoke particle every frame
                    particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), Vector3.Zero);
#endif
                }
                   
                else if (GAMESTATE.Equals(ACTIVESTATE))
                {
#if WINDOWS_PHONE
                    if (particle == explosionParticles)
                        particle.AddParticles(Project(fireMarkerNode1.WorldTransformation.Translation));
#else
                    // Add 1 smoke particle every frame
                    particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), Vector3.Zero);
#endif
                }
            }
        }
        /// <summary>
        /// Get a random point on a circle
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Vector3 RandomPointOnCircle(Vector3 pos)
        {
            const float radius = 12.5f;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector3(x * radius + pos.X, y * radius + pos.Y, pos.Z);

        }


#if WINDOWS_PHONE
        /// <summary>
        /// Projects object space coordinate to screen coordinate
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector2 Project(Vector3 position)
        {
            
            if(position == fireMarkerNode1.WorldTransformation.Translation)
                scene.CameraNode = arCameraNode;

            Vector3 pos2d = State.Device.Viewport.Project(position, State.ProjectionMatrix,
               State.ViewMatrix, Matrix.Identity);

                return new Vector2(pos2d.X, pos2d.Y);
            
           
        }
#endif

        private void LoadContent(ContentManager content)
        {
            sampleFont = content.Load<SpriteFont>("Sample");
        }

        public void Dispose()
        {
            scene.Dispose();
        }

        private void RegisterFires()
        {
            have_registered_fires = false;
            fire_matrices = new Dictionary<int, Matrix>();
            fire_positions = new List<Vector3>();
            if (fireMarkerNode1.MarkerFound)
            {
                fire_matrices.Add(fireMarkerNode1.ID, fireMarkerNode1.WorldTransformation);
            }

            if (fireMarkerNode2.MarkerFound)
            {
                fire_matrices.Add(fireMarkerNode2.ID, fireMarkerNode2.WorldTransformation);
            }

            if (fireMarkerNode3.MarkerFound)
            {
                fire_matrices.Add(fireMarkerNode3.ID, fireMarkerNode3.WorldTransformation);
            }

            navigator = new MiniMapNavigation(fire_matrices);
            navigator.set_scale();
            navigator.init_positions();
            fire_positions = navigator.get_positions();

            firesRepTransNode = new TransformNode();

            foreach (Vector3 vector in fire_positions)
            {
                TransformNode fire_displacement = new TransformNode()
                {
                    Translation = vector
                };
                GeometryNode fire_sphere = new GeometryNode("Fire Sphere");
                fire_sphere.Model = new Sphere(10, 5, 5);
                Material sphereMat = new Material();
                sphereMat.Diffuse = Color.Red.ToVector4();
                sphereMat.Specular = Color.White.ToVector4();
                sphereMat.SpecularPower = 10;
                fire_sphere.Material = sphereMat;
                firesRepTransNode.AddChild(fire_displacement);
                fire_displacement.AddChild(fire_sphere);
            }
            have_registered_fires = true;
        }

        public void Update(TimeSpan elapsedTime, bool isActive)
        {
            if (GAMESTATE.Equals(COUNTDOWNSTATE))
            {
                countdown -= (float)elapsedTime.TotalSeconds;
                if ((float)Math.Round(countdown) <= 0)
                {
                    UI2DRenderer.WriteText(new Vector2(0, 0), "START", Color.White,
                                           sampleFont, new Vector2(5, 5),
                                           GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Center);
                    if ((float)Math.Round(countdown) < -2)
                    {

                        RegisterFires();
                        GAMESTATE = ACTIVESTATE;
                    }
                }
                else
                {
                    UI2DRenderer.WriteText(new Vector2(0, 0), ((float)Math.Round(countdown)).ToString(), Color.White, sampleFont, new Vector2(5, 5),
                                   GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Center);

                }
            }
            if (GAMESTATE.Equals(ACTIVESTATE) || GAMESTATE.Equals(INGAMEHELPSTATE))
            {
                if (GAMESTATE.Equals(ACTIVESTATE))
                {
                    timer += (float)elapsedTime.TotalSeconds;
                }
                UI2DRenderer.WriteText(new Vector2(0, 0), ((float)Math.Round(timer)).ToString() + " Seconds", Color.White, sampleFont,
                                  GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Top);
            }
            if (GAMESTATE.Equals(CONFIRMATION))
            {
                UI2DRenderer.WriteText(new Vector2(0, 0), EXITGAMECONFIRMATION, Color.White, sampleFont, new Vector2(2, 2),
                                 GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Center);
            }
           
            scene.Update(elapsedTime, false, isActive);
        }

        public void Draw(TimeSpan elapsedTime)
        {

            isDrawingMiniMap = true;



            boxTransNode.Enabled = false;
            fireEffectNode.Enabled = false;
            fireEffectNode2.Enabled = false;
            fireEffectNode3.Enabled = false;


            smokeEffectNode.Enabled = false;
            smokeEffectNode2.Enabled = false;
            smokeEffectNode3.Enabled = false;
            noMarkerNode.RemoveChild(noTransform);
            yesMarkerNode.RemoveChild(yesTransform);
            helpMarkerNode.RemoveChild(helpTransform);
            startMarkerNode.RemoveChild(exitTransform);
            waterMarkerNode.RemoveChild(hoseTransNode);
            valveNode.RemoveChild(plusTransNode);
            valveNode.RemoveChild(minusTransNode);



            scene.SceneRenderTarget = vrViewRenderTarget;
            scene.BackgroundColor = Color.DarkOliveGreen;
            // Set the scene background size to be the size of the VR scene viewport
            scene.BackgroundBound = vrViewRect;
            // Set the camera to be the VR camera
            scene.CameraNode = vrCameraNode;
            // Render the marker board and camera representation in VR scene
            vrCameraRepTransNode.Enabled = true;
            vrCameraRepNode.Enabled = true;

            //UPDATE THE PYRAMID
            if (have_registered_fires)
            {
            
                scene.RootNode.AddChild(firesRepTransNode);

                if (fireMarkerNode1.MarkerFound)
                {
                    navigator.set_position(fireMarkerNode1.ID, Matrix.Invert(fireMarkerNode1.WorldTransformation));
                    vrCameraRepTransNode.WorldTransformation = navigator.get_position();
                }
                if (fireMarkerNode2.MarkerFound)
                {
                    navigator.set_position(fireMarkerNode2.ID, Matrix.Invert(fireMarkerNode2.WorldTransformation));
                    vrCameraRepTransNode.WorldTransformation = navigator.get_position();
                }
                if (fireMarkerNode3.MarkerFound)
                {
                    navigator.set_position(fireMarkerNode3.ID, Matrix.Invert(fireMarkerNode3.WorldTransformation));
                    vrCameraRepTransNode.WorldTransformation = navigator.get_position();
                }
            }
    
            scene.BackgroundTexture = null;
            // Re-traverse the scene graph since we have modified it, and render the VR scene 
            scene.RenderScene(false, true);
            
            
            //scene.Draw(elapsedTime, false);
         
            // Get the world Transformation of one marker so that if it is rotated we dont 
            // Perform a menu selection
            isDrawingMiniMap = false;
            scene.SceneRenderTarget = arViewRenderTarget;
            scene.BackgroundColor = Color.Black;
            scene.BackgroundBound = arViewRect;
            scene.CameraNode = arCameraNode;
            vrCameraRepNode.Enabled = false;
            scene.BackgroundTexture = videoTexture;

            fireEffectNode.Enabled = true;
            fireEffectNode2.Enabled = true;
            fireEffectNode3.Enabled = true;

            smokeEffectNode.Enabled = true;
            smokeEffectNode2.Enabled = true;
            smokeEffectNode3.Enabled = true;

            if (have_registered_fires)
            {
                scene.RootNode.RemoveChild(firesRepTransNode);
            }
            noMarkerNode.AddChild(noTransform);
            yesMarkerNode.AddChild(yesTransform);
            helpMarkerNode.AddChild(helpTransform);
            startMarkerNode.AddChild(exitTransform);

            waterMarkerNode.AddChild(hoseTransNode);
            valveNode.AddChild(plusTransNode);
            valveNode.AddChild(minusTransNode);

            Matrix markerMat1 = startMarkerNode.WorldTransformation;
            Quaternion rot1 = new Quaternion();
            Vector3 sc1 = new Vector3();
            Vector3 tran1 = new Vector3();
            markerMat1.Decompose(out sc1, out rot1, out tran1);

            Matrix startCameraView = startMarkerNode.WorldTransformation;
            // We need to do this for both markers.
            Quaternion rot3 = new Quaternion();
            Vector3 sc3 = new Vector3();
            Vector3 tran3 = new Vector3();
            startCameraView.Decompose(out sc3, out rot3, out tran3);
            Vector3 screenStartPosition = viewport.Project(tran3, State.ProjectionMatrix,
                                            State.ViewMatrix, Matrix.Identity);

            Matrix yesCameraView = noMarkerNode.WorldTransformation;

            // We need to do this for both markers.
            Quaternion rot4 = new Quaternion();
            Vector3 sc4 = new Vector3();
            Vector3 tran4 = new Vector3();
            yesCameraView.Decompose(out sc4, out rot4, out tran4);
            Vector3 yesScreenPosition = viewport.Project(tran4, State.ProjectionMatrix,
                                          State.ViewMatrix, Matrix.Identity);


            // We need to do this for both markers.
            Matrix markerMat2 = helpMarkerNode.WorldTransformation;
            Quaternion rot2 = new Quaternion();
            Vector3 sc2 = new Vector3();
            Vector3 tran2 = new Vector3();
            markerMat2.Decompose(out sc2, out rot2, out tran2);
            // If the menu was not visible and all menu markers are now visible and the rotation is not NAN, then we set the 
            // menu to its visibility state.
            if (MENUSTATE.Equals(MENUSTATEOFF) && startMarkerNode.MarkerFound && helpMarkerNode.MarkerFound 
               && tran4.Z > -400 && tran4.Z < -300)
            {
                MENUSTATE = MENUSTATEON;
            }
            else if (!(tran4.Z > -400 && tran4.Z < -300))
            {

                MENUSTATE = MENUSTATEOFF;
            }
            else if (noMarkerNode.MarkerFound && !yesMarkerNode.MarkerFound && MENUSTATE.Equals(MENUSTATEON) && GAMESTATE.Equals(CONFIRMATION))
            {
                GAMESTATE = PREVIOUS;
                MENUSTATE = MENUSTATEOFF;

            }
            else if (!noMarkerNode.MarkerFound && yesMarkerNode.MarkerFound && MENUSTATE.Equals(MENUSTATEON) && GAMESTATE.Equals(CONFIRMATION))
            {

                PREVIOUS = "null";
                GAMESTATE = IDLESTATE;
                MENUSTATE = MENUSTATEOFF;

            }
            // If one marker is visible and the other markers are visible then we select the marker that is obstructed
            else if (helpMarkerNode.MarkerFound && !startMarkerNode.MarkerFound && MENUSTATE.Equals(MENUSTATEON))
            {

                // If you are currently in the idle state and you start the game you move into the active state
                // which represents the game state
                if (GAMESTATE.Equals(IDLESTATE))
                {
                    // Set the timer equal to 0
                    timer = 0;
                    countdown = 5;
                    GAMESTATE = COUNTDOWNSTATE; //ACTIVESTATE;

                }
                // If you are in either the active state or the scope state and you wish to exit the game early, you can
                // exit the game which brings you back to the idle state.
                else if (GAMESTATE.Equals(ACTIVESTATE) || GAMESTATE.Equals(SCOPESTATE) || GAMESTATE.Equals(COUNTDOWNSTATE))
                {
                    PREVIOUS = GAMESTATE;
                    GAMESTATE = CONFIRMATION;
                }
                // One a selection is made the menu needs to be placed back to its original state to avoid several selections
                // at a time.
                MENUSTATE = MENUSTATEOFF;

            }
            // If marker 3 obstructed then this triggers the help UIs to trigger the help menu
            else if (startMarkerNode.MarkerFound && !helpMarkerNode.MarkerFound && MENUSTATE.Equals(MENUSTATEON))
            {

                pageNumber = 1;
                // If the game is in its idle state and the help is selected, we go into idle help mode
                if (!HELPSTATE && GAMESTATE.Equals(IDLESTATE))
                {
                    HELPSTATE = true;
                    GAMESTATE = IDLEHELPSTATE;
                }
                    // If the game is in its idle help state and we exit, it should return to the idle state
                else if (GAMESTATE.Equals(IDLEHELPSTATE))
                {
                    HELPSTATE = false;
                    GAMESTATE = IDLESTATE;
                }
                // If the game is not idle and help is requested we go into the in game help
                if (!HELPSTATE && GAMESTATE.Equals(ACTIVESTATE))
                {
                    HELPSTATE = true;
                    GAMESTATE = INGAMEHELPSTATE;
                }
                    // If the game quits help during in game help, we want to return to the game
                else if (GAMESTATE.Equals(INGAMEHELPSTATE))
                {
                    HELPSTATE = false;
                    GAMESTATE = ACTIVESTATE;
                }

                MENUSTATE = MENUSTATEOFF;
            }

            if (!yesMarkerNode.MarkerFound && noMarkerNode.MarkerFound && (GAMESTATE.Equals(IDLEHELPSTATE) || GAMESTATE.Equals(INGAMEHELPSTATE)) && !MANYPAGES)
            {

                pageNumber = pageNumber + 1;
                if (pageNumber > 12)
                    pageNumber = 1;
                scene.UIRenderer.Remove2DComponent(helpPanel);
                helpScreen.Texture = GlobalContent.Load<Texture2D>("Help" + pageNumber.ToString());
                scene.UIRenderer.Add2DComponent(helpPanel);
                MANYPAGES = true;
            }
            else if (yesMarkerNode.MarkerFound && !noMarkerNode.MarkerFound && (GAMESTATE.Equals(IDLEHELPSTATE) || GAMESTATE.Equals(INGAMEHELPSTATE)) && !MANYPAGES)
            {
                pageNumber = pageNumber - 1;
                if (pageNumber < 1)
                    pageNumber = 12;
                scene.UIRenderer.Remove2DComponent(helpPanel);
                helpScreen.Texture = GlobalContent.Load<Texture2D>("Help" + pageNumber.ToString());
                scene.UIRenderer.Add2DComponent(helpPanel);
                MANYPAGES = true;
            }
            else if (yesMarkerNode.MarkerFound && noMarkerNode.MarkerFound && (GAMESTATE.Equals(IDLEHELPSTATE) || GAMESTATE.Equals(INGAMEHELPSTATE)) && MANYPAGES)
            {
                MANYPAGES = false;
            }


            // Always show the UI.
            if (MENUSTATE.Equals(MENUSTATEON))
            {
                showUI();
            }
            

            // If both the valve marker node and nozzle pointer node are vissible then we want to be able to shoot the water.
            if (valveNode.MarkerFound && waterMarkerNode.MarkerFound)
            {

                Matrix mat = valveNode.WorldTransformation *
                    Matrix.Invert(waterMarkerNode.WorldTransformation);

                // containers for breaking down the matrix
                Quaternion angle = new Quaternion();
                Vector3 scale = new Vector3();
                Vector3 translation = new Vector3();
                mat.Decompose(out scale, out angle, out translation);

                waterPressure = angle.Z;


            }
            boxTransNode.Enabled = true;

            scene.Draw(elapsedTime, false);

            
            
            // Adjust the viewport to be centered
            arViewRect.X += viewport.X;
            vrViewRect.X += viewport.X;

            // Set the render target back to the frame buffer
            State.Device.SetRenderTarget(null);
            
            State.Device.Clear(Color.Black);

            // Render the two textures rendered on the render targets
            State.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            State.SharedSpriteBatch.Draw(arViewRenderTarget, arViewRect, Color.White);
            State.SharedSpriteBatch.Draw(vrViewRenderTarget, vrViewRect, Color.White);
            State.SharedSpriteBatch.End();

            // Reset the adjustments
            arViewRect.X -= viewport.X;
            vrViewRect.X -= viewport.X;

     
             
        }
    }
}
