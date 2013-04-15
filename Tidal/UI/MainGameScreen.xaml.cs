//////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2013, Pete Vasiliauskas <pete@nofailgames.com>
//
// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
// SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR
// IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Tidal.Game;
using Tidal.Utils;

namespace Tidal.UI
{
    /// <summary>
    /// Class to run all of the game UI.
    /// 
    /// TODO: Much of the game logic is in here too and could be separated out.
    /// </summary>
    public partial class MainGameScreen : UserControl
    {
        /// <summary>
        /// Default constructor for VS designer
        /// </summary>
        public MainGameScreen()
        {
            gameState = GameState.Loading;
            movingCard = NO_CARD_MOVING;
            cardsInPlay = new CardsInPlay();
            cardsDisplayed = new CardsDisplayed(cardsInPlay);
            InitializeComponent();
        }

        /// <summary>
        /// Normal implementation constructor
        /// </summary>
        /// <param name="root">Reference to the root control</param>
        /// <param name="diff">Chosen difficulty level of the game</param>
        public MainGameScreen(RootContainer root, GameDifficulty diff) :
            this()
        {
            rootContainer = root;
            gameDiff = diff;
            DifficultyText.Text = gameDiff.ToString();
        }

        /// <summary>
        /// Loaded, initialize the controls, the deck, and start a deal
        /// </summary>
        private void MainGameControl_Loaded(object sender, RoutedEventArgs e)
        {
            cardsInPlay.Initialize32Deck();

            StatusText.Text = "Ready.";

            RectangleGeometry clipRect = new RectangleGeometry();
            clipRect.Rect = new Rect((double)BgRect.GetValue(Canvas.LeftProperty),
                                (double)BgRect.GetValue(Canvas.TopProperty),
                                BgRect.Width,
                                BgRect.Height);
            clipRect.RadiusX = BgRect.RadiusX;
            clipRect.RadiusY = BgRect.RadiusY;
            GameCanvas.Clip = clipRect;

            HowToPlayControl htp = new HowToPlayControl();
            GameCanvas.Children.Add(htp);
            htp.SetValue(Canvas.TopProperty, 0.0);
            htp.SetValue(Canvas.LeftProperty, HowToPlayControl.HIDDENX);
            Canvas.SetZIndex(htp, 65535);

            AboutControl about = new AboutControl();
            GameCanvas.Children.Add(about);
            about.SetValue(Canvas.TopProperty, AboutControl.HIDDENY);
            about.SetValue(Canvas.LeftProperty, AboutControl.HIDDENX);
            Canvas.SetZIndex(about, 65534);

            Canvas.SetZIndex(TitleButton, 65533);
            Canvas.SetZIndex(RedealButton, 65532);

            // Start a deal
            RedealButton_Click(null, null);
        }

        /// <summary>
        /// Re-initialize the deck and deal again
        /// </summary>
        private void RedealButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameState != GameState.Loading && gameState != GameState.GameOver)
            {
                rootContainer.Stats.GameLost(gameDiff);
            }

            StopAndRemoveAllAnimations();
            int animDelay = 0;
            cardsDisplayed.RemoveAllAndReshuffle(GameCanvas);
            for (int i = 0; i < 7; i++)
            {
                cardsDisplayed.DrawOne(GameCanvas,
                    new Point(CardsDisplayed.LEFTMOSTCARDX + i * CardsDisplayed.CARDSPACING, CardsDisplayed.HUMANCARDY),
                    false,
                    (animDelay++) * 50);
                cardsDisplayed.DrawOne(GameCanvas,
                    new Point(CardsDisplayed.LEFTMOSTCARDX + i * CardsDisplayed.CARDSPACING, CardsDisplayed.AICARDY),
                    true,
                    (animDelay++) * 50);
            }
            cardsDisplayed.ArrangeCards(GameCanvas);  // Initial sort for them, the animations will all work out
            setsPassed.Clear();
            UpdateCardCountText();
            gameState = GameState.YourTurn;
            StatusText.Text = "Your turn, select any sets of three cards that you wish to pass.  Place all of the sets to pass in the center.";
            AcceptButton.Content = "Pass None";
            AcceptButton.IsEnabled = true;
        }

        /// <summary>
        /// Return to title screen
        /// </summary>
        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameState != GameState.Loading && gameState != GameState.GameOver)
            {
                rootContainer.Stats.GameLost(gameDiff);
            }

            StopAndRemoveAllAnimations();
            rootContainer.SwitchControl(new TitleScreen(rootContainer));
        }

        /// <summary>
        /// Main user action button.  Depending on state, can mean pass, send cards,
        /// accept cards.
        /// </summary>
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameState == GameState.YourTurn)
            {
                List<PlayCard> cards = cardsDisplayed.GetCardsInRect(InnerRectRect);

                if ((cards.Count % 3) == 0)
                {
                    AcceptButton.IsEnabled = false;
                    if (cards.Count > 0)
                    {
                        setsPassed = CardSet.FormMaxSets(cards);
                        if (setsPassed.Count * 3 == cards.Count)
                        {
                            StatusText.Text = "Valid sets, AI is thinking about which cards to take.";
                            gameState = GameState.AIReceiveCards;
                            AIReceiveCards();
                            StatusText.Text = "AI takes cards.";
                        }
                        else
                        {
                            setsPassed.Clear();
                            StatusText.Text = "The sets that you are trying to pass are not valid.  Try some other sets of three cards.";
                            return;
                        }
                    }
                    else
                    {
                        StatusText.Text = "AI's turn.";
                    }
                    cardsDisplayed.ArrangeCards(GameCanvas);
                    UpdateCardCountText();
                    gameState = GameState.AITurn;
                    AITurn();
                    if (gameState != GameState.GameOver)
                    {
                        if (setsPassed.Count > 0)
                        {
                            StatusText.Text = "The AI has passed you the cards in the center.  For each set of three, either move two cards to your hand or place a fourth matching card from your hand on it, then click on Accept.";
                            AcceptButton.Content = "Accept Cards";
                            AcceptButton.IsEnabled = false;
                        }
                        else
                        {
                            StatusText.Text = "The AI did not pass you any cards.  Click on Accept to continue.";
                            AcceptButton.Content = "Accept None";
                            AcceptButton.IsEnabled = true;
                        }
                        gameState = GameState.YouReceiveCards;
                    }
                }
                else
                {
                    StatusText.Text = "The sets that you are trying to pass are not valid.  Try some other sets of three cards.";
                }
            }
            else if (gameState == GameState.YouReceiveCards)
            {
                List<DisplayCard> passed = cardsDisplayed.GetDisplayCardsInRect(InnerRectRect);
                if (IsValidPassResolution(passed))
                {
                    // Extras can go away
                    foreach (DisplayCard c in passed)
                    {
                        cardsDisplayed.Discard(GameCanvas, c.Card.CardID);
                    }

                    // Start of turn, draw a card.
                    Point rt = cardsDisplayed.RightmostCardBelow(InnerRectRect.Bottom);
                    if (rt.X < CardsDisplayed.LEFTMOSTCARDX - CardsDisplayed.CARDSPACING)
                    {
                        rt = new Point(CardsDisplayed.LEFTMOSTCARDX - CardsDisplayed.CARDSPACING, CardsDisplayed.HUMANCARDY);
                    }
                    else if (rt.X > Width - 100)
                    {
                        rt.X = Width - 100;
                        if (rt.Y > Height - 100)
                        {
                            rt.Y = Height - 75;
                        }
                    }
                    rt.X += CardsDisplayed.CARDSPACING;
                    cardsDisplayed.DrawOne(GameCanvas, rt, false, 0);
                    UpdateCardCountText();
                    int yourCards = cardsDisplayed.CountFaceUpCards();
                    if (yourCards < 14)
                    {
                        StatusText.Text = "Your turn, select any sets of three cards that you wish to pass.  Place all of the sets to pass in the center.";
                        AcceptButton.Content = "Pass None";
                        cardsDisplayed.ArrangeCards(GameCanvas);
                        gameState = GameState.YourTurn;
                    }
                    else
                    {
                        StatusText.Text = string.Format("You have {0} cards, you lose.", yourCards);
                        AcceptButton.Content = "Pass None";
                        gameState = GameState.GameOver;
                        AcceptButton.IsEnabled = false;
                        rootContainer.Stats.GameLost(gameDiff);
                        ShowLoseAnimation();
                    }
                }
            }
        }

        /// <summary>
        /// Check the cards passed by the AI to see if the user has validly accepted them
        /// </summary>
        /// <param name="remainingCards">The list of cards remaining on the table</param>
        /// <returns>true if the passed cards can be validly accepted</returns>
        private bool IsValidPassResolution(List<DisplayCard> remainingCards)
        {
            bool isValid = true;

            if (setsPassed.Count == 0 && remainingCards.Count > 0)
            {
                // No sets were passed, you can't put any of your cards on the table
                isValid = false;
            }
            else
            {
                // Game rules: you must accept exactly 2 of cards passed,
                // or add a 4th card from your hand to cancel them
                foreach (CardSet set in setsPassed)
                {
                    int nSetCardsOnTable = 0;
                    if (remainingCards.ContainsCardID(set.Card1))
                        nSetCardsOnTable++;
                    if (remainingCards.ContainsCardID(set.Card2))
                        nSetCardsOnTable++;
                    if (remainingCards.ContainsCardID(set.Card3))
                        nSetCardsOnTable++;

                    if (nSetCardsOnTable == 0 || nSetCardsOnTable == 2)
                    {
                        // All or 1 of the passed cards were pulled from the table.  No good.
                        isValid = false;
                        break;
                    }
                    else if (nSetCardsOnTable == 3)
                    {
                        // All of the passed cards are on the table... did we match a 4th up?
                        bool valid4th = false;
                        foreach (DisplayCard c in remainingCards)
                        {
                            // TODO: PV: This doesn't handle if a single card is a valid canceller
                            //           for more than 1 set on the board (ie: used twice)
                            if (set.IsValid4th(c.Card.CardID) && !setsPassed.ContainsCardID(c.Card.CardID))
                            {
                                valid4th = true;
                                break;
                            }
                        }
                        isValid = valid4th;
                    }
                    // else
                    //   n == 1 is valid, the user accepted 2 of the passed cards from the set, continue
                }
            }

            return isValid;
        }

        /// <summary>
        /// Take the AI turn.
        /// </summary>
        private void AITurn()
        {
            // Start of turn, draw a card.
            Point rt = cardsDisplayed.RightmostFaceDownCard();
            rt.X += CardsDisplayed.CARDSPACING;
            cardsDisplayed.DrawOne(GameCanvas, rt, true, 0);
            UpdateCardCountText();
            int aiCardsCount = cardsDisplayed.CountFaceDownCards();
            if (aiCardsCount < 14)
            {
                List<PlayCard> aiCards = cardsDisplayed.GetAllFaceDown();
                List<CardSet> setsCouldPass = CardSet.FormMaxSets(aiCards);
                if (gameDiff == GameDifficulty.Easy)
                {
                    // Easy only passes 1 set at a time
                    while (setsCouldPass.Count > 1)
                    {
                        setsCouldPass.RemoveAt(1);
                    }
                }
                else if (gameDiff == GameDifficulty.Medium)
                {
                    // Medium passes all sets, all the time
                    // (so do nothing with the could pass)
                }
                else // if (gameDiff == GameDifficulty.Hard)
                {
                    // Hard passes maximum sets when nearing danger
                    if (aiCards.Count <= 10)
                    {
                        setsCouldPass.Clear();
                    }
                }
                setsPassed = setsCouldPass;
                if (setsPassed.Count > 0)
                {
                    cardsDisplayed.MoveCardsFaceUpToRect(GameCanvas, setsPassed, InnerRectRect);
                }
            }
            else
            {
                setsPassed.Clear();
                UpdateCardCountText();
                StatusText.Text = string.Format("The AI has {0} cards, you win.", aiCardsCount);
                gameState = GameState.GameOver;
                rootContainer.Stats.GameWon(gameDiff);
                ShowWinAnimation();
            }
        }

        /// <summary>
        /// The user has passed something to the AI, have it accept it
        /// </summary>
        private void AIReceiveCards()
        {
            Point rt = cardsDisplayed.RightmostFaceDownCard();
            rt.X += CardsDisplayed.CARDSPACING;
            Random rnd = new Random((int)DateTime.Now.Ticks);
            List<PlayCard> aiCards = cardsDisplayed.GetAllFaceDown();
            List<int> cardsToDiscard = new List<int>();
            List<int> cardsAIKeeps = new List<int>();

            if (gameDiff == GameDifficulty.Easy)
            {
                // Easy does not cancel sets (that's why they call it easy)
                foreach (CardSet set in setsPassed)
                {
                    int k = rnd.Next(3);
                    if (k == 0)
                    {
                        cardsToDiscard.Add(set.Card1);
                        cardsAIKeeps.Add(set.Card2);
                        cardsAIKeeps.Add(set.Card3);
                    }
                    else if (k == 1)
                    {
                        cardsAIKeeps.Add(set.Card1);
                        cardsToDiscard.Add(set.Card2);
                        cardsAIKeeps.Add(set.Card3);
                    }
                    else
                    {
                        cardsAIKeeps.Add(set.Card1);
                        cardsAIKeeps.Add(set.Card2);
                        cardsToDiscard.Add(set.Card3);
                    }
                }
            }
            else if (gameDiff == GameDifficulty.Medium)
            {
                // Medium cancels sets whenever possible
                foreach (CardSet set in setsPassed)
                {
                    bool setCancelled = false;
                    foreach (PlayCard c in aiCards)
                    {
                        if (set.IsValid4th(c.CardID))
                        {
                            cardsToDiscard.Add(set.Card1);
                            cardsToDiscard.Add(set.Card2);
                            cardsToDiscard.Add(set.Card3);
                            cardsToDiscard.Add(c.CardID);
                            aiCards.Remove(c);
                            setCancelled = true;
                            break;
                        }
                    }
                    if (!setCancelled)
                    {
                        int k = rnd.Next(3);
                        if (k == 0)
                        {
                            cardsToDiscard.Add(set.Card1);
                            cardsAIKeeps.Add(set.Card2);
                            cardsAIKeeps.Add(set.Card3);
                        }
                        else if (k == 1)
                        {
                            cardsAIKeeps.Add(set.Card1);
                            cardsToDiscard.Add(set.Card2);
                            cardsAIKeeps.Add(set.Card3);
                        }
                        else
                        {
                            cardsAIKeeps.Add(set.Card1);
                            cardsAIKeeps.Add(set.Card2);
                            cardsToDiscard.Add(set.Card3);
                        }
                    }
                }
            }
            else // if (gameDiff == GameDifficulty.Hard)
            {
                // Hard only cancels sets to prevent a loss
                int numSetsAbleToTake = (12 - aiCards.Count) / 2;
                int numSetsCancelled = 0;
                // TODO: Hard can be smarter about which cards it picks to cancel
                foreach (CardSet set in setsPassed)
                {
                    bool setCancelled = false;
                    if (numSetsAbleToTake < setsPassed.Count - numSetsCancelled)
                    {
                        foreach (PlayCard c in aiCards)
                        {
                            if (set.IsValid4th(c.CardID))
                            {
                                cardsToDiscard.Add(set.Card1);
                                cardsToDiscard.Add(set.Card2);
                                cardsToDiscard.Add(set.Card3);
                                cardsToDiscard.Add(c.CardID);
                                aiCards.Remove(c);
                                numSetsCancelled++;
                                setCancelled = true;
                                break;
                            }
                        }
                    }
                    if (!setCancelled)
                    {
                        int k = rnd.Next(3);
                        if (k == 0)
                        {
                            cardsToDiscard.Add(set.Card1);
                            cardsAIKeeps.Add(set.Card2);
                            cardsAIKeeps.Add(set.Card3);
                        }
                        else if (k == 1)
                        {
                            cardsAIKeeps.Add(set.Card1);
                            cardsToDiscard.Add(set.Card2);
                            cardsAIKeeps.Add(set.Card3);
                        }
                        else
                        {
                            cardsAIKeeps.Add(set.Card1);
                            cardsAIKeeps.Add(set.Card2);
                            cardsToDiscard.Add(set.Card3);
                        }
                    }
                }
            }

            // Handle movements
            foreach (int c in cardsToDiscard)
            {
                // TODO: PV: Animate discard
                cardsDisplayed.Discard(GameCanvas, c);
            }
            foreach (int c in cardsAIKeeps)
            {
                cardsDisplayed.TurnFaceDown(GameCanvas, cardsDisplayed.GetCardByID(c));
                cardsDisplayed.MoveToTop(GameCanvas, cardsDisplayed.GetCardByID(c));
                cardsDisplayed.AnimateCardMove(GameCanvas, cardsDisplayed.GetCardByID(c), rt);
                rt.X += CardsDisplayed.CARDSPACING;
            }
        }

        /// <summary>
        /// Update the display of total cards in AI and user hands
        /// </summary>
        void UpdateCardCountText()
        {
            int nAICards = cardsDisplayed.CountFaceDownCards();
            int nPlayerCards = cardsDisplayed.CountFaceUpCards();
            string sAIText = string.Format("Cards: {0}/13", nAICards);
            string sPlayerText = string.Format("Cards: {0}/13", nPlayerCards);
            AICardCountText.Text = sAIText;
            PlayerCardCountText.Text = sPlayerText;

            int nDrawCards = cardsInPlay.DrawPileCount();
            string sDrawText = string.Format("{0} cards in deck", nDrawCards);
            CardsRemaining.Text = sDrawText;

            if (nAICards <= 10)
                AICardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x55, 0x44));
            else if (nAICards == 11)
                AICardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x88, 0x00));
            else if (nAICards == 12)
                AICardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0x66, 0x00));
            else if (nAICards == 13)
                AICardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0x33, 0x00));
            else
                AICardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x00, 0x00));

            if (nPlayerCards <= 10)
                PlayerCardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x55, 0x44));
            else if (nPlayerCards == 11)
                PlayerCardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x88, 0x00));
            else if (nPlayerCards == 12)
                PlayerCardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0x66, 0x00));
            else if (nPlayerCards == 13)
                PlayerCardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0x33, 0x00));
            else
                PlayerCardCountBar.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x00, 0x00));
        }

        #region GameCanvas Events...

        /// <summary>
        /// Handle dragging a card around
        /// </summary>
        private void GameCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (movingCard != NO_CARD_MOVING)
            {
                DisplayCard card = cardsDisplayed.GetCardByID(movingCard);
                if (card != null)
                {
                    card.CenterLocation = e.GetPosition(this);
                }
            }
        }

        /// <summary>
        /// Handle picking a card up to drag it
        /// </summary>
        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (gameState == GameState.YourTurn || gameState == GameState.YouReceiveCards)
            {
                DisplayCard card = cardsDisplayed.HitTest(e.GetPosition(this));
                if (card != null && card.Card != null && card.CardState != DisplayCard.CardStates.FaceDown)
                {
                    movingCard = card.Card.CardID;
                    cardsDisplayed.MoveToTop(GameCanvas, card);
                }
                GameCanvas_MouseMove(sender, e); // Force card movement update
            }
        }

        /// <summary>
        /// Handle dropping a card
        /// </summary>
        private void GameCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            movingCard = NO_CARD_MOVING;

            if (gameState == GameState.YourTurn || gameState == GameState.YouReceiveCards)
            {
                List<DisplayCard> passing = cardsDisplayed.GetDisplayCardsInRect(InnerRectRect);
                if (gameState == GameState.YourTurn)
                {
                    if (passing.Count == 0)
                    {
                        AcceptButton.Content = "Pass None";
                        AcceptButton.IsEnabled = true;
                    }
                    else
                    {
                        AcceptButton.Content = "Pass Cards";
                        if ((passing.Count % 3) == 0)
                        {
                            AcceptButton.IsEnabled = true;
                        }
                        else
                        {
                            AcceptButton.IsEnabled = false;
                        }
                    }
                }
                else if (gameState == GameState.YouReceiveCards)
                {
                    if (IsValidPassResolution(passing))
                    {
                        AcceptButton.IsEnabled = true;
                    }
                    else
                    {
                        AcceptButton.IsEnabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// If the mouse leaves the game area, let's leave the cards in it.
        /// </summary>
        private void GameCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            movingCard = NO_CARD_MOVING;
        }

        #endregion

        #region Animations...

        /// <summary>
        /// Show an animation when the user wins the game.
        /// </summary>
        private void ShowWinAnimation()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            int animDelayMs = 1000;
            Storyboard sb = null;
            foreach (DisplayCard card in cardsDisplayed.Cards)
            {
                // Setup animation
                TransformGroup t = new TransformGroup();
                RotateTransform rot = new RotateTransform();
                rot.CenterX = CardImages.CARD_WIDTH / 2;
                rot.CenterY = CardImages.CARD_HEIGHT / 2;
                t.Children.Add(rot);
                t.Children.Add(new TranslateTransform());
                ScaleTransform s = new ScaleTransform();
                s.CenterX = CardImages.CARD_WIDTH / 2;
                s.CenterY = CardImages.CARD_HEIGHT / 2;
                t.Children.Add(s);
                card.Image.RenderTransform = t;
                Duration durFull = new Duration(TimeSpan.FromMilliseconds(700 + animDelayMs));
                Duration dur = new Duration(TimeSpan.FromMilliseconds(700));
                DoubleAnimation d1 = new DoubleAnimation();
                DoubleAnimation d2 = new DoubleAnimation();
                DoubleAnimation d3 = new DoubleAnimation();
                DoubleAnimation d4 = new DoubleAnimation();
                DoubleAnimation d5 = new DoubleAnimation();
                DoubleAnimation d6 = new DoubleAnimation();
                d1.Duration = dur;
                d2.Duration = dur;
                d3.Duration = dur;
                d4.Duration = dur;
                d5.Duration = dur;
                d6.Duration = dur;
                d1.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d2.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d3.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d4.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d5.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d6.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                sb = new Storyboard();
                sb.Duration = durFull;
                sb.Children.Add(d1);
                sb.Children.Add(d2);
                sb.Children.Add(d3);
                sb.Children.Add(d4);
                sb.Children.Add(d5);
                sb.Children.Add(d6);
                Storyboard.SetTarget(d1, card.Image);
                Storyboard.SetTarget(d2, card.Image);
                Storyboard.SetTarget(d3, card.Image);
                Storyboard.SetTarget(d4, card.Image);
                Storyboard.SetTarget(d5, card.Image);
                Storyboard.SetTarget(d6, card.Image);
                Storyboard.SetTargetProperty(d1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(d2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)"));
                Storyboard.SetTargetProperty(d3, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
                Storyboard.SetTargetProperty(d4, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(d5, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(ScaleTransform.ScaleY)"));
                Storyboard.SetTargetProperty(d6, new PropertyPath("Opacity"));
                d1.To = rnd.Next(-200, 200);
                d2.To = rnd.Next(-200, 200);
                d3.To = 720;
                d4.To = 3;
                d5.To = 3;
                d6.To = 0;
                if (GameCanvas.Resources.Contains("win_anim_sb" + card.Card.CardID.ToString()))
                {
                    GameCanvas.Resources.Remove("win_anim_sb" + card.Card.CardID.ToString());
                }
                GameCanvas.Resources.Add("win_anim_sb" + card.Card.CardID.ToString(), sb);
                sb.Begin();
                animDelayMs += 200;
            }
            if (sb != null)
            {
                sb.Completed += new EventHandler(WinAnimation_Completed);
            }
        }

        /// <summary>
        /// Clean up when the animation completes.
        /// </summary>
        void WinAnimation_Completed(object sender, EventArgs e)
        {
            StopAndRemoveAllAnimations();
            cardsDisplayed.RemoveAllAndReshuffle(GameCanvas);
        }

        /// <summary>
        /// Show an animation when the user loses the game.
        /// </summary>
        private void ShowLoseAnimation()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            int animDelayMs = 1000;
            Storyboard sb = null;
            foreach (DisplayCard card in cardsDisplayed.Cards)
            {
                // Setup animation
                TransformGroup t = new TransformGroup();
                RotateTransform rot = new RotateTransform();
                rot.CenterX = CardImages.CARD_WIDTH / 2;
                rot.CenterY = CardImages.CARD_HEIGHT / 2;
                t.Children.Add(rot);
                t.Children.Add(new TranslateTransform());
                ScaleTransform s = new ScaleTransform();
                s.CenterX = CardImages.CARD_WIDTH / 2;
                s.CenterY = CardImages.CARD_HEIGHT / 2;
                t.Children.Add(s);
                card.Image.RenderTransform = t;
                Duration durFull = new Duration(TimeSpan.FromMilliseconds(700 + animDelayMs));
                Duration dur = new Duration(TimeSpan.FromMilliseconds(700));
                DoubleAnimation d1 = new DoubleAnimation();
                DoubleAnimation d2 = new DoubleAnimation();
                DoubleAnimation d3 = new DoubleAnimation();
                DoubleAnimation d4 = new DoubleAnimation();
                DoubleAnimation d5 = new DoubleAnimation();
                DoubleAnimation d6 = new DoubleAnimation();
                d1.Duration = dur;
                d2.Duration = dur;
                d3.Duration = dur;
                d4.Duration = dur;
                d5.Duration = dur;
                d6.Duration = dur;
                d1.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d2.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d3.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d4.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d5.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d6.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                sb = new Storyboard();
                sb.Duration = durFull;
                sb.Children.Add(d1);
                sb.Children.Add(d2);
                sb.Children.Add(d3);
                sb.Children.Add(d4);
                sb.Children.Add(d5);
                sb.Children.Add(d6);
                Storyboard.SetTarget(d1, card.Image);
                Storyboard.SetTarget(d2, card.Image);
                Storyboard.SetTarget(d3, card.Image);
                Storyboard.SetTarget(d4, card.Image);
                Storyboard.SetTarget(d5, card.Image);
                Storyboard.SetTarget(d6, card.Image);
                Storyboard.SetTargetProperty(d1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(d2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)"));
                Storyboard.SetTargetProperty(d3, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
                Storyboard.SetTargetProperty(d4, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(d5, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(ScaleTransform.ScaleY)"));
                Storyboard.SetTargetProperty(d6, new PropertyPath("Opacity"));
                d1.To = 400 - card.X;
                d2.To = 400 - card.Y;
                d3.To = 180;
                d4.To = .1;
                d5.To = .1;
                d6.To = 0;
                if (GameCanvas.Resources.Contains("lose_anim_sb" + card.Card.CardID.ToString()))
                {
                    GameCanvas.Resources.Remove("lose_anim_sb" + card.Card.CardID.ToString());
                }
                GameCanvas.Resources.Add("lose_anim_sb" + card.Card.CardID.ToString(), sb);
                sb.Begin();
                animDelayMs += 200;
            }
            if (sb != null)
            {
                sb.Completed += new EventHandler(LoseAnimation_Completed);
            }
        }

        /// <summary>
        /// Clean up when the animation completes.
        /// </summary>
        void LoseAnimation_Completed(object sender, EventArgs e)
        {
            StopAndRemoveAllAnimations();
            cardsDisplayed.RemoveAllAndReshuffle(GameCanvas);
        }

        /// <summary>
        /// Clean up after any animations that are still going on.
        /// </summary>
        private void StopAndRemoveAllAnimations()
        {
            List<string> sbToRemove = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                if (GameCanvas.Resources.Contains("card_move_sb" + i.ToString()))
                {
                    sbToRemove.Add("card_move_sb" + i.ToString());
                    (GameCanvas.Resources["card_move_sb" + i.ToString()] as Storyboard).Stop();
                }
                if (GameCanvas.Resources.Contains("card_drawone_sb" + i.ToString()))
                {
                    sbToRemove.Add("card_drawone_sb" + i.ToString());
                    (GameCanvas.Resources["card_drawone_sb" + i.ToString()] as Storyboard).Stop();
                }
                if (GameCanvas.Resources.Contains("win_anim_sb" + i.ToString()))
                {
                    sbToRemove.Add("win_anim_sb" + i.ToString());
                    (GameCanvas.Resources["win_anim_sb" + i.ToString()] as Storyboard).Stop();
                }
                if (GameCanvas.Resources.Contains("lose_anim_sb" + i.ToString()))
                {
                    sbToRemove.Add("lose_anim_sb" + i.ToString());
                    (GameCanvas.Resources["lose_anim_sb" + i.ToString()] as Storyboard).Stop();
                }
            }
            foreach (string sb in sbToRemove)
            {
                GameCanvas.Resources.Remove(sb);
            }
        }

        #endregion

        /// <summary>
        /// The rectangle of the "table" area, used for passing cards.
        /// </summary>
        private Rect InnerRectRect
        {
            get
            {
                // Part of the background image, so return a static value
                return new Rect(150.0, 150.0, 400.0, 200.0);
            }
        }

        /// <summary>
        /// enum to track what state the game is in
        /// </summary>
        private enum GameState
        {
            Loading,
            YourTurn,
            AIReceiveCards,
            AITurn,
            YouReceiveCards,
            GameOver
        }

        private const int NO_CARD_MOVING = -1;
        private int movingCard = NO_CARD_MOVING;
        private List<CardSet> setsPassed = new List<CardSet>();
        private GameState gameState = GameState.Loading;
        private GameDifficulty gameDiff = GameDifficulty.Easy;
        private RootContainer rootContainer = null;
        private CardsInPlay cardsInPlay = null;
        private CardsDisplayed cardsDisplayed = null;
    }
}
