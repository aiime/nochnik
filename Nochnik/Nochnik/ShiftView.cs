using System.Collections.Generic;

namespace Nochnik
{
    static class ShiftView
    {
        static readonly int SHIFTS_IN_CIRCLE = 72;
        static readonly int SHIFTS_IN_SQUARE = 12;
        static readonly int MAX_STICKS = 12;
        static readonly int MAX_SQUARES = 6;
        static readonly int MAX_CIRCLES = 6;
        static readonly int CURRENT_STICK = 1;
        static readonly int CURRENT_SQUARE = 1;
        static readonly int CURRENT_CIRCLE = 1;
        static readonly int GAP_BETWEEN_ELEMENTS = 3;
        static readonly int DOWN_ALIGNMENT = 6; // Необходимо, чтобы приподнять круги и квадраты к палочкам.

        /// <summary>Начало координат снизу слева, т.к. графические элементы выравниваются к нижней части.</summary>
        /// <param name="userShiftView_x">Начало координат снизу слева.</param>
        /// <param name="userShiftView_y">Начало координат снизу слева.</param>
        /// <returns></returns>
        public static List<WallpaperPart> UpdateUserShiftView(User user, int userShiftView_x, int userShiftView_y)
        {
            List<WallpaperPart> userShiftView = new List<WallpaperPart>();

            int shiftsLeft = user.ShiftsLeft;
            int shiftsCompleted = user.ShiftsCompleted;

            int wholeCirclesLeft = shiftsLeft / SHIFTS_IN_CIRCLE;
            int wholeSquaresLeft = (shiftsLeft - (wholeCirclesLeft * SHIFTS_IN_CIRCLE)) / SHIFTS_IN_SQUARE;
            int wholeShiftsLeft = shiftsLeft - (wholeCirclesLeft * SHIFTS_IN_CIRCLE + wholeSquaresLeft * SHIFTS_IN_SQUARE);

            int wholeCirclesCompleted = MAX_CIRCLES - wholeCirclesLeft - CURRENT_CIRCLE;
            int wholeSquaresCompleted = MAX_SQUARES - wholeSquaresLeft - CURRENT_SQUARE;
            int wholeShiftsCompleted = MAX_STICKS - wholeShiftsLeft - CURRENT_STICK;

            // Add sticks.
            int stickZoneStart_x = userShiftView_x + GAP_BETWEEN_ELEMENTS + Properties.Resources.circle.Width + 
                                                     GAP_BETWEEN_ELEMENTS + Properties.Resources.square.Width + 
                                                     GAP_BETWEEN_ELEMENTS;
            int stickZoneStart_y = userShiftView_y - 9;
            List<WallpaperPart> sticks = CreateUserSticks(stickZoneStart_x, stickZoneStart_y, wholeShiftsLeft, wholeShiftsCompleted);
            userShiftView.AddRange(sticks);

            // Add squares.
            int squareZoneStart_x = userShiftView_x + GAP_BETWEEN_ELEMENTS + Properties.Resources.circle.Width + 
                                                      GAP_BETWEEN_ELEMENTS;
            int squareZoneStart_y = userShiftView_y - DOWN_ALIGNMENT - 9;
            List<WallpaperPart> squares = CreateUserSquares(squareZoneStart_x, squareZoneStart_y, wholeSquaresLeft, wholeSquaresCompleted);
            userShiftView.AddRange(squares);

            // Add circles.
            int circleZoneStart_x = userShiftView_x + GAP_BETWEEN_ELEMENTS;
            int circleZoneStart_y = userShiftView_y - DOWN_ALIGNMENT - 9;
            List<WallpaperPart> circles = CreateUserCircles(circleZoneStart_x, circleZoneStart_y, wholeCirclesLeft, wholeCirclesCompleted);
            userShiftView.AddRange(circles);

            return userShiftView;
        }

        /// <summary>Начало координат снизу слева, т.к. графические элементы выравниваются к нижней части.</summary>
        /// <param name="stickZoneStart_x">Начало координат снизу слева.</param>
        /// <param name="stickZoneStart_y">Начало координат снизу слева.</param>
        static List<WallpaperPart> CreateUserSticks(int stickZoneStart_x, int stickZoneStart_y, int futureShiftsNumber, int completedShiftsNumber)
        {
            List<WallpaperPart> sticks = new List<WallpaperPart>();

            int stickWidth = Properties.Resources.stick.Width;
            int stickHeight = Properties.Resources.stick.Height;

            int stick_x;
            int stick_y;
            // Добавляем палочки будущих смен.
            for (int i = 0; i < futureShiftsNumber; i++)
            {
                stick_x = stickZoneStart_x;
                stick_y = stickZoneStart_y - (stickHeight * i) - (GAP_BETWEEN_ELEMENTS * i);
                WallpaperPart futureShiftStick = new WallpaperPart(Properties.Resources.stick, stick_x, stick_y, stickWidth, stickHeight);
                sticks.Add(futureShiftStick);
            }

            // Добавляем палочку текущей смены.
            stick_x = stickZoneStart_x;
            stick_y = stickZoneStart_y - (stickHeight * futureShiftsNumber) - (GAP_BETWEEN_ELEMENTS * futureShiftsNumber);
            WallpaperPart currentShiftStick = new WallpaperPart(Properties.Resources.stick_orange, stick_x, stick_y, stickWidth, stickHeight);
            sticks.Add(currentShiftStick);

            //// Добавялем палочки завершённых смен.
            //for (int i = futureShiftsNumber + CURRENT_STICK, j = 0; j < completedShiftsNumber; i++, j++)
            //{
            //    stick_x = stickZoneStart_x;
            //    stick_y = stickZoneStart_y - (stickHeight * i) - (GAP_BETWEEN_ELEMENTS * i);
            //    WallpaperPart completedShiftStick = new WallpaperPart(Properties.Resources.stick_gray, stick_x, stick_y, stickWidth, stickHeight);
            //    sticks.Add(completedShiftStick);
            //}

            return sticks;
        }

        /// <summary>Начало координат снизу слева, т.к. графические элементы выравниваются к нижней части.</summary>
        /// <param name="squareZoneStart_x">Начало координат снизу слева.</param>
        /// <param name="squareZoneStart_y">Начало координат снизу слева.</param>
        static List<WallpaperPart> CreateUserSquares(int squareZoneStart_x, int squareZoneStart_y, int futureSquaresNumber, int completedSquaresNumber)
        {
            List<WallpaperPart> squares = new List<WallpaperPart>();

            int squareWidth = Properties.Resources.square.Width;
            int squareHeight = Properties.Resources.square.Height;

            for (int i = 0; i < futureSquaresNumber; i++)
            {
                int square_x = squareZoneStart_x;
                int square_y = squareZoneStart_y - (squareHeight * i) - (GAP_BETWEEN_ELEMENTS * i);
                WallpaperPart square = new WallpaperPart(Properties.Resources.square, square_x, square_y, squareWidth, squareHeight);
                squares.Add(square);

                // Если все квадраты были добавленны, то добавляем последний текущий квадрат.
                if (i + 1 == futureSquaresNumber)
                {
                    int currentSquare_x = squareZoneStart_x;
                    int currentSquare_y = squareZoneStart_y - (squareHeight * (i + 1)) - (GAP_BETWEEN_ELEMENTS * (i + 1));
                    WallpaperPart currentSquare = new WallpaperPart(Properties.Resources.square_orange, currentSquare_x, currentSquare_y, squareWidth, squareHeight);
                    squares.Add(currentSquare);
                }
            }

            return squares;
        }

        /// <summary>Начало координат снизу слева, т.к. графические элементы выравниваются к нижней части.</summary>
        /// <param name="circleZoneStart_x">Начало координат снизу слева.</param>
        /// <param name="circleZoneStart_y">Начало координат снизу слева.</param>
        static List<WallpaperPart> CreateUserCircles(int circleZoneStart_x, int circleZoneStart_y, int circleNumber, int completedCirclesNumber)
        {
            List<WallpaperPart> circles = new List<WallpaperPart>();

            int circleWidth = Properties.Resources.circle.Width;
            int circleHeight = Properties.Resources.circle.Height;

            for (int i = 0; i < circleNumber; i++)
            {
                int circle_x = circleZoneStart_x;
                int circle_y = circleZoneStart_y - (circleHeight * i) - (GAP_BETWEEN_ELEMENTS * i);
                WallpaperPart circle = new WallpaperPart(Properties.Resources.circle, circle_x, circle_y, circleWidth, circleHeight);
                circles.Add(circle);

                // Если все круги были добавленны, то добавляем последний текущий круг.
                if (i + 1 == circleNumber)
                {
                    int currentСircle_x = circleZoneStart_x;
                    int currentСircle_y = circleZoneStart_y - (circleHeight * (i + 1)) - (GAP_BETWEEN_ELEMENTS * (i + 1));
                    WallpaperPart currentСircle = new WallpaperPart(Properties.Resources.circle_orange, currentСircle_x, currentСircle_y, circleWidth, circleHeight);
                    circles.Add(currentСircle);
                }
            }

            return circles;
        }
    }
}
