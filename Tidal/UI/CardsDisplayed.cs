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
using System.Windows.Media;
using System.Windows.Media.Animation;
using Tidal.Game;
using Tidal.Utils;

namespace Tidal.UI
{
    /// <summary>
    /// Handles display and movement aspects of a card
    /// </summary>
    public class CardsDisplayed
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inPlay">A reference to the current cards in play tracker</param>
        public CardsDisplayed(CardsInPlay inPlay)
        {
            cardsInPlay = inPlay;
            Cards = new List<DisplayCard>();
        }

        /// <summary>
        /// All of the displayed cards
        /// </summary>
        public List<DisplayCard> Cards { get; private set; }

        /// <summary>
        /// Get a card from the draw pile, reshuffling if necessary.  Animate it to
        /// the desired location.
        /// </summary>
        /// <param name="canvas">Where to show the card</param>
        /// <param name="location">Where to animate the card to</param>
        /// <param name="faceDown">True if the card show not be showing (AI card)</param>
        /// <param name="animDelayMs">Time to wait before starting the animation</param>
        /// <returns>The ID of the card drawn</returns>
        public int DrawOne(Canvas canvas, Point location, bool faceDown, int animDelayMs)
        {
            PlayCard card = cardsInPlay.DrawOneReshuffleDiscard();
            if (card != null)
            {
                Image img = null;
                if (faceDown)
                    img = CardImages.Instance.GetBgCardImage();
                else
                    img = CardImages.Instance.GetCardImage(card.CardID);
                DisplayCard dcard = new DisplayCard(img, card, faceDown);
                Cards.Insert(0, dcard);
                canvas.Children.Add(dcard.Image);
                dcard.Location = location;

                // Setup animation
                TransformGroup t = new TransformGroup();
                RotateTransform rot = new RotateTransform();
                rot.CenterX = CardImages.CARD_WIDTH / 2;
                rot.CenterY = CardImages.CARD_HEIGHT / 2;
                rot.Angle = -360;
                t.Children.Add(rot);
                TranslateTransform trans = new TranslateTransform();
                trans.X = 0 - CardImages.CARD_WIDTH - location.X - 250;
                trans.Y = 250 - (CardImages.CARD_HEIGHT / 2) - location.Y;
                t.Children.Add(trans);
                img.RenderTransform = t;
                Duration durFull = new Duration(TimeSpan.FromMilliseconds(300 + animDelayMs));
                Duration dur = new Duration(TimeSpan.FromMilliseconds(300));
                DoubleAnimation d1 = new DoubleAnimation();
                DoubleAnimation d2 = new DoubleAnimation();
                DoubleAnimation d3 = new DoubleAnimation();
                d1.Duration = dur;
                d2.Duration = dur;
                d3.Duration = dur;
                d1.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d2.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                d3.BeginTime = TimeSpan.FromMilliseconds(animDelayMs);
                Storyboard sb = new Storyboard();
                sb.Duration = durFull;
                sb.Children.Add(d1);
                sb.Children.Add(d2);
                sb.Children.Add(d3);
                Storyboard.SetTarget(d1, img);
                Storyboard.SetTarget(d2, img);
                Storyboard.SetTarget(d3, img);
                Storyboard.SetTargetProperty(d1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(d2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)"));
                Storyboard.SetTargetProperty(d3, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
                d1.To = 0;
                d2.To = 0;
                d3.To = 0;
                if (canvas.Resources.Contains("card_drawone_sb" + card.CardID.ToString()))
                {
                    canvas.Resources.Remove("card_drawone_sb" + card.CardID.ToString());
                }
                canvas.Resources.Add("card_drawone_sb" + card.CardID.ToString(), sb);
                sb.Begin();

                return card.CardID;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Animates a card moving from its current location to a new location.
        /// </summary>
        /// <param name="canvas">Where to show the card</param>
        /// <param name="card">The card to move</param>
        /// <param name="newLocation">The card's destination</param>
        public void AnimateCardMove(Canvas canvas, DisplayCard card, Point newLocation)
        {
            TranslateTransform trans = new TranslateTransform();
            trans.X = card.X - newLocation.X;
            trans.Y = card.Y - newLocation.Y;
            card.Image.RenderTransform = trans;
            card.Location = newLocation;
            Duration dur = new Duration(TimeSpan.FromMilliseconds(300));
            DoubleAnimation d1 = new DoubleAnimation();
            DoubleAnimation d2 = new DoubleAnimation();
            d1.Duration = dur;
            d2.Duration = dur;
            Storyboard sb = new Storyboard();
            sb.Duration = dur;
            sb.Children.Add(d1);
            sb.Children.Add(d2);
            Storyboard.SetTarget(d1, card.Image);
            Storyboard.SetTarget(d2, card.Image);
            Storyboard.SetTargetProperty(d1, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTargetProperty(d2, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            d1.To = 0;
            d2.To = 0;
            if (canvas.Resources.Contains("card_move_sb" + card.Card.CardID.ToString()))
            {
                canvas.Resources.Remove("card_move_sb" + card.Card.CardID.ToString());
            }
            canvas.Resources.Add("card_move_sb" + card.Card.CardID.ToString(), sb);
            sb.Begin();
        }

        /// <summary>
        /// Get a card by its ID
        /// </summary>
        /// <param name="id">The ID</param>
        /// <returns>The card, or null if not found</returns>
        public DisplayCard GetCardByID(int id)
        {
            foreach (DisplayCard card in Cards)
            {
                if (card.Card != null && card.Card.CardID == id)
                    return card;
            }
            return null;
        }

        /// <summary>
        /// Returns a card that is at the given point, or null if none
        /// </summary>
        /// <param name="point">Where to look</param>
        /// <returns>The card, or null if none</returns>
        public DisplayCard HitTest(Point point)
        {
            foreach (DisplayCard card in Cards)
            {
                if (card.Image != null &&
                    card.X <= point.X &&
                    card.Y <= point.Y &&
                    card.X + card.Image.ActualWidth >= point.X &&
                    card.Y + card.Image.ActualHeight >= point.Y)
                {
                    return card;
                }
            }
            return null;
        }

        /// <summary>
        /// Move a card to the top of the Z-order
        /// </summary>
        /// <param name="canvas">Where to show the card</param>
        /// <param name="card">The card to move</param>
        public void MoveToTop(Canvas canvas, DisplayCard card)
        {
            // TODO: Probably should also sync the CardsInPlay... so other clients draw correctly
            Cards.Remove(card);
            canvas.Children.Remove(card.Image);
            Cards.Insert(0, card);
            canvas.Children.Add(card.Image);
        }

        /// <summary>
        /// Remove all cards from display and reshuffle the deck
        /// </summary>
        /// <param name="canvas">The container of the shown cards</param>
        public void RemoveAllAndReshuffle(Canvas canvas)
        {
            foreach (DisplayCard card in Cards)
            {
                canvas.Children.Remove(card.Image);
            }
            Cards.Clear();
            cardsInPlay.Initialize32Deck();
        }

        /// <summary>
        /// Get all cards in a given rectangle
        /// </summary>
        /// <param name="rect">Where to look</param>
        /// <returns>The list of cards, or an empty rectangle if none</returns>
        public List<PlayCard> GetCardsInRect(Rect rect)
        {
            List<PlayCard> cardsInRect = new List<PlayCard>();
            foreach (DisplayCard card in Cards)
            {
                if (card.Card != null)
                {
                    Rect cr = new Rect((double)card.Image.GetValue(Canvas.LeftProperty),
                                          (double)card.Image.GetValue(Canvas.TopProperty),
                                          card.Image.ActualWidth,
                                          card.Image.ActualHeight);
                    if (rect.Contains(cr))
                    {
                        cardsInRect.Add(card.Card);
                    }
                }
            }
            return cardsInRect;
        }

        /// <summary>
        /// Get all cards in a given rectangle
        /// </summary>
        /// <param name="rect">Where to look</param>
        /// <returns>The list of cards, or an empty rectangle if none</returns>
        public List<DisplayCard> GetDisplayCardsInRect(Rect rect)
        {
            List<DisplayCard> cardsInRect = new List<DisplayCard>();
            foreach (DisplayCard card in Cards)
            {
                if (card.Card != null)
                {
                    Rect cr = new Rect((double)card.Image.GetValue(Canvas.LeftProperty),
                                          (double)card.Image.GetValue(Canvas.TopProperty),
                                          card.Image.ActualWidth,
                                          card.Image.ActualHeight);
                    if (rect.Contains(cr))
                    {
                        cardsInRect.Add(card);
                    }
                }
            }
            return cardsInRect;
        }

        /// <summary>
        /// Get the right-most card below a certain point
        /// </summary>
        /// <param name="y">Below this point</param>
        /// <returns>The location of the right-most card</returns>
        public Point RightmostCardBelow(double y)
        {
            Point rt = new Point(0, 0);
            foreach (DisplayCard card in Cards)
            {
                if ((double)card.Image.GetValue(Canvas.TopProperty) > y &&
                    (double)card.Image.GetValue(Canvas.LeftProperty) > rt.X)
                {
                    rt.X = (double)card.Image.GetValue(Canvas.LeftProperty);
                    rt.Y = (double)card.Image.GetValue(Canvas.TopProperty);
                }
            }
            return rt;
        }

        /// <summary>
        /// Get the right-most face down card.
        /// </summary>
        /// <returns>The location of the right-most card</returns>
        public Point RightmostFaceDownCard()
        {
            Point rt = new Point(0, 0);
            foreach (DisplayCard card in Cards)
            {
                if (card.CardState == DisplayCard.CardStates.FaceDown &&
                    (double)card.Image.GetValue(Canvas.LeftProperty) > rt.X)
                {
                    rt.X = (double)card.Image.GetValue(Canvas.LeftProperty);
                    rt.Y = (double)card.Image.GetValue(Canvas.TopProperty);
                }
            }
            return rt;
        }

        /// <summary>
        /// How many cards are face down?
        /// </summary>
        /// <returns>How many cards are face down</returns>
        public int CountFaceDownCards()
        {
            int count = 0;
            foreach (DisplayCard card in Cards)
            {
                if (card.CardState == DisplayCard.CardStates.FaceDown)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// How many cards are face up?
        /// </summary>
        /// <returns>How many cards are face up</returns>
        public int CountFaceUpCards()
        {
            int count = 0;
            foreach (DisplayCard card in Cards)
            {
                if (card.CardState != DisplayCard.CardStates.FaceDown)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Discard a card, removing it from view
        /// </summary>
        /// <param name="canvas">Where the card is</param>
        /// <param name="cardID">Which card to remove</param>
        public void Discard(Canvas canvas, int cardID)
        {
            DisplayCard card = GetCardByID(cardID);
            if (card != null)
            {
                canvas.Children.Remove(card.Image);
                card.Card.Pile = PlayCard.CardPile.DiscardPile;
                Cards.Remove(card);
            }
        }

        /// <summary>
        /// Flip a card face down
        /// </summary>
        /// <param name="canvas">Where the card is</param>
        /// <param name="card">The card to flip</param>
        public void TurnFaceDown(Canvas canvas, DisplayCard card)
        {
            card.CardState = DisplayCard.CardStates.FaceDown;
            Point loc = card.Location;
            canvas.Children.Remove(card.Image);
            card.Image = CardImages.Instance.GetBgCardImage();
            canvas.Children.Add(card.Image);
            card.Location = loc;
        }

        /// <summary>
        /// Flip a card face up
        /// </summary>
        /// <param name="canvas">Where the card is</param>
        /// <param name="card">The card to flip</param>
        /// <param name="state">The state the face-up card should be in</param>
        private void TurnFaceUp(Canvas canvas, DisplayCard card, DisplayCard.CardStates state)
        {
            card.CardState = state;
            Point loc = card.Location;
            canvas.Children.Remove(card.Image);
            card.Image = CardImages.Instance.GetCardImage(card.Card.CardID);
            canvas.Children.Add(card.Image);
            card.Location = loc;
            MoveToTop(canvas, card);
        }

        /// <summary>
        /// Get all the face down cards
        /// </summary>
        /// <returns>A list of all the face down cards</returns>
        public List<PlayCard> GetAllFaceDown()
        {
            List<PlayCard> cardsFaceDown = new List<PlayCard>();
            foreach (DisplayCard card in Cards)
            {
                if (card.Card != null && card.CardState == DisplayCard.CardStates.FaceDown)
                {
                    cardsFaceDown.Add(card.Card);
                }
            }
            return cardsFaceDown;
        }

        /// <summary>
        /// Move the AI's passed cards face up to the given rectangle
        /// </summary>
        /// <param name="canvas">Where the cards are</param>
        /// <param name="cards">The cards to move</param>
        /// <param name="rect">Where to place the cards</param>
        public void MoveCardsFaceUpToRect(Canvas canvas, List<CardSet> cards, Rect rect)
        {
            double x = rect.Left + 25;
            double y = rect.Top + (rect.Height / 2) - (CardImages.CARD_HEIGHT / 2);
            foreach (CardSet set in cards)
            {
                DisplayCard d = GetCardByID(set.Card1);
                TurnFaceUp(canvas, d, DisplayCard.CardStates.FaceUpPassFromAI);
                AnimateCardMove(canvas, d, new Point(x, y));
                x += 25;
                d = GetCardByID(set.Card2);
                TurnFaceUp(canvas, d, DisplayCard.CardStates.FaceUpPassFromAI);
                AnimateCardMove(canvas, d, new Point(x, y));
                x += 25;
                d = GetCardByID(set.Card3);
                TurnFaceUp(canvas, d, DisplayCard.CardStates.FaceUpPassFromAI);
                AnimateCardMove(canvas, d, new Point(x, y));
                x += 75;
            }
        }

        /// <summary>
        /// Arrange all the displayed cards so that they show up neatly
        /// </summary>
        /// <param name="canvas">Where the cards are</param>
        public void ArrangeCards(Canvas canvas)
        {
            List<DisplayCard> listAICards = new List<DisplayCard>();
            List<DisplayCard> listHumanCards = new List<DisplayCard>();
            foreach (DisplayCard d in Cards)
            {
                if (d.CardState == DisplayCard.CardStates.FaceDown)
                {
                    listAICards.InsertInXOrder(d);
                }
                else
                {
                    listHumanCards.Add(d);
                }
            }
            // Let's just sort (backwards)... it's likely what the user wants.
            // Don't sort the AI cards, you could deduce what they have when they go missing.
            listHumanCards.Sort();
            listHumanCards.Reverse();
            int x = LEFTMOSTCARDX;
            foreach (DisplayCard d in listAICards)
            {
                MoveToTop(canvas, d);
                d.X = x;
                d.Y = AICARDY;
                x += CARDSPACING;
            }
            x = LEFTMOSTCARDX;
            foreach (DisplayCard d in listHumanCards)
            {
                MoveToTop(canvas, d);
                d.X = x;
                d.Y = HUMANCARDY;
                x += CARDSPACING;
            }
        }

        private CardsInPlay cardsInPlay = null;

        /// <summary>
        /// X position of AI/Human hands
        /// </summary>
        public const int LEFTMOSTCARDX = 100;
        /// <summary>
        /// Y position of AI cards
        /// </summary>
        public const int AICARDY = 33;
        /// <summary>
        /// Y position of human cards
        /// </summary>
        public const int HUMANCARDY = 370;
        /// <summary>
        /// Space between cards
        /// </summary>
        public const int CARDSPACING = 25;
    }
}
