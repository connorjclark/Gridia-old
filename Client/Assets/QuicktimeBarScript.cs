using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gridia;
using UnityEngine.UI;
using Random = System.Random;

public class QuicktimeBarScript : MonoBehaviour
{
    private const int AttackRow = 0;
    private const int DefenseRow = 1;

    public class Zone
    {
        public int Row, Column, Width, Status;
        public KeyCode Key;
    }

    public const int CellsHorizontal = 50;
    public const int CellsVertical = 2;
    public float PercentBufferFromEdgeOfScreen = 0.05f;
    public List<Zone> Zones;
    public float Tick;
    public float TotalTime = 1.25f;
    public Text AttackText, DefenseText;
    public Action<int, int> OnFinish;

    public int Misses;

    public int AttackPoints
    {
        get
        {
            return Zones
                .Where(z => z.Row == AttackRow)
                .Aggregate(0, (acc, zone) =>
                {
                    if (zone.Status == 1)
                    {
                        return acc + 10;
                    }
                    else if (zone.Status == -1)
                    {
                        return acc - 15;
                    }
                    else
                    {
                        return acc;
                    }
                }) - Misses*15;
        }
    }

    public int DefensePoints
    {
        get
        {
            return Math.Max(-50, Zones
                .Where(z => z.Row == DefenseRow)
                .Aggregate(0, (acc, zone) =>
                {
                    if (zone.Status == 1)
                    {
                        return acc + 15;
                    }
                    else if (zone.Status == -1)
                    {
                        return acc - 10;
                    }
                    else
                    {
                        return acc;
                    }
                }) - Misses*10);
        }
    }

    private Random random = new Random();

    // http://stackoverflow.com/a/3122628/2788187
    private int[] GetUniformPartition(int input, int parts)
    {
        if (input <= 0 || parts <= 0)
            throw new ArgumentException("invalid input or parts");
        if (input < MinUniformPartition(parts))
            throw new ArgumentException("input is to small");

        int[] partition = new int[parts];
        int sum = 0;
        for (int i = 0; i < parts - 1; i++)
        {
            int max = input - MinUniformPartition(parts - i - 1) - sum;
            partition[i] = random.Next(parts - i, max);
            sum += partition[i];
        }
        partition[parts - 1] = input - sum; // last 
        return partition;
    }

    private int MinUniformPartition(int n)
    {
        return n*n - 1;
    }

    private void Start()
    {
        Tick = Misses = 0;
        Zones = new List<Zone>();

        var numCellsToSkipAtStart = 2;
        var numCellsToSkipAtEnd = 0;

        var partitionLength = CellsHorizontal - numCellsToSkipAtStart - numCellsToSkipAtEnd;

        for (var y = 0; y < CellsVertical; y++)
        {
            var partition = GetUniformPartition(partitionLength, 7);
            var x = numCellsToSkipAtStart;

            for (var i = 0; i < partition.Length; i++)
            {
                if (i%2 == 1)
                {
                    var key = (KeyCode) ((int) KeyCode.Alpha1 + random.Next(3));
                    var w = Math.Max(1, partition[i]/2);
                    var zone = new Zone {Row = y, Column = x, Width = w, Key = key};
                    Zones.Add(zone);
                }
                x += partition[i];
            }
        }
    }

    private bool sent = false;

    public void Update()
    {
        Tick = Tick + Time.deltaTime/TotalTime;
        if (Tick >= 2)
        {
            // Start();
            Destroy(gameObject);
            return;
        }
        else if (Tick >= 1)
        {
            if (!sent)
            {
                Debug.Log(AttackPoints);
                Locator.Get<ConnectionToGridiaServerHandler>().SetDefense(DefensePoints);
                Locator.Get<ConnectionToGridiaServerHandler>().Attack(AttackPoints);
            }
            sent = true;
            return;
        }

        var activeTick = (int) (Tick*CellsHorizontal);
        var anyHit = false;
        var keyPressed = Input.inputString != "";
        Action<Action<Zone>> onActiveZones = action =>
        {
            Zones.Where(z => z.Column <= activeTick && z.Column + z.Width >= activeTick && z.Status != -1).All(zone =>
            {
                action(zone);
                return true;
            });
        };

        onActiveZones(zone =>
        {
            if (Input.GetKeyDown(zone.Key) && zone.Status == 0)
            {
                zone.Status = 1;
                anyHit = true;
            }
        });

        if (keyPressed && !anyHit)
        {
            var anyAtAll = false;
            onActiveZones(zone =>
            {
                anyAtAll = true;
                zone.Status = -1;
            });
            if (!anyAtAll)
            {
                Misses += 1;
            }
        }

        if (AttackText != null) AttackText.text = AttackPoints.ToString();
        if (DefenseText != null) DefenseText.text = DefensePoints + "%";
    }

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;

        var activeColor = Color.yellow;
        var inactiveColor = Color.grey;
        var successColor = Color.green;
        var failureColor = Color.red;
        var cellWidth = (1 - PercentBufferFromEdgeOfScreen * 2) * Screen.width / CellsHorizontal;
        var cellHeight = Screen.height  * 0.05f;
        var startX = PercentBufferFromEdgeOfScreen * Screen.width;
        var activeTick = Tick <= 1 ? (int) (Tick*CellsHorizontal) : (int) ((2 - Tick)*CellsHorizontal);
        const int border = 2;

        var barWidth = cellWidth*CellsHorizontal;
        var barHeight = cellHeight*CellsVertical;
        GUI.BeginGroup(new Rect(transform.position.x + startX, Screen.height - transform.position.y - barHeight, barWidth, barHeight));
        
        DrawRectangle(new Rect(0, 0, barWidth, barHeight), inactiveColor);
        DrawRectangle(new Rect(activeTick*cellWidth, 0, cellWidth, barHeight), Tick <= 1 ? activeColor : Color.blue);
        DrawRectBorder(new Rect(0, 0, barWidth, barHeight), border, Color.black);
        DrawRectangle(new Rect(0, (barHeight - border) / 2, barWidth, border), Color.black);

        var style = new GUIStyle {fontSize = (int) (cellHeight*0.9)};

        Zones.ForEach(zone =>
        {
            var rect = new Rect(zone.Column*cellWidth, zone.Row*(cellHeight - border), cellWidth*zone.Width, cellHeight);
            if (zone.Status == 1)
            {
                DrawRectangle(rect, successColor);
            }
            else if (zone.Status == -1)
            {
                DrawRectangle(rect, failureColor);
            }
            DrawRectBorder(rect, 1, activeColor);
            if (zone.Key != KeyCode.None)
            {
                var text = zone.Key.ToShortString();
                var x = rect.x + (rect.width - style.CalcSize(new GUIContent(text)).x)/2;
                var w = rect.width/2;
                var textRect = new Rect(x, rect.y, w, rect.height);
                GUI.Label(textRect, text, style);
            }
        });
        
        GUI.EndGroup();
    }

    private void DrawRectBorder(Rect rect, int border, Color color)
    {
        DrawRectangle(new Rect(rect.x, rect.y, rect.width, border), color);
        DrawRectangle(new Rect(rect.x, rect.y + rect.height - border, rect.width, border), color);
        DrawRectangle(new Rect(rect.x + rect.width - border, rect.y, border, rect.height), color);
        DrawRectangle(new Rect(rect.x, rect.y, border, rect.height), color);
    }

    private void DrawRectangle(Rect rect, Color color)
    {   
        GridiaConstants.GUIDrawSelector(rect, color);
    }
}
