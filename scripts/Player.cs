using System;
using System.Collections.Generic;
using Godot;

public class Player : KinematicBody2D
{
    Vector2 velocity = new Vector2();
    int acceleration = 10;
    const int speed = 160;
    bool calledRocket = false;

    public override void _PhysicsProcess(float delta)
    {
        LookAt(GetGlobalMousePosition());
        Rotate(Mathf.Pi / 2);
        //var input = Input.GetVector("left", "right", "up", "down");
        var input = new Vector2();
        if (Input.IsActionPressed("left"))
        {
            input.x = -1;
        }
        if (Input.IsActionPressed("right"))
        {
            input.x = 1;
        }
        if (Input.IsActionPressed("up"))
        {
            input.y = -1;
        }
        if (Input.IsActionPressed("down"))
        {
            input.y = 1;
        }

        if (Input.IsActionPressed("launch"))
        {
            ReparentRocketHand(ref calledRocket);
        }

        input = input.Normalized();

        if (input == Vector2.Zero)
        {
            velocity = Vector2.Zero;
        }
        else
        {
            velocity += input * acceleration * speed;
            velocity = velocity.Normalized() * speed;
        }
        MoveAndSlide(velocity); // no delta because MAS uses it internally

        var UIControlNode = GetNode<Control>("UICanvasLayer/UIControlNode");
        var label = UIControlNode.GetChild<Label>(0);
        var inventoryNode = GetNode<Inventory>("Inventory");
        var listData = inventoryNode.Storage;
        var strArray = new List<string>();
        foreach (var e in listData)
            strArray.Add(e.ToString());
        var listString = string.Join("\n", strArray);

        label.Text = $"{input}\n{velocity}\n{Position}\n\nslots:{listData.Count}\n{listString}";
    }

    public void ReparentRocketHand(ref bool called)
    {
        if (called)
        {
            return;
        }
        var child = GetChild<Hand>(2);

        var gp = child.GlobalPosition;
        var gr = child.GlobalRotation;

        RemoveChild(child);

        GetNode<Main>("/root/Main").AddChild(child);

        child.GlobalPosition = gp;
        child.GlobalRotation = gr;

        calledRocket = true;
    }
}
