
using analyzer;

public class CompilerVisitor : LanguageBaseVisitor<ValueWrapper>
{
    public ValueWrapper defaultValue = new VoidValue();
    public string output = "";
    public Environment currentEnviroment;

    public CompilerVisitor()
    {
        currentEnviroment = new Environment(null);
        Embeded.Generate(currentEnviroment);
    }


    //VisitProgram
    public override ValueWrapper VisitProgram(LanguageParser.ProgramContext context)
    {
        foreach (var dcl in context.dcl())
        {
            Visit(dcl);
        }
        return defaultValue;
    }

    //VisitVarDcl
    public override ValueWrapper VisitVarDcl(LanguageParser.VarDclContext context)
    {
        string id = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        currentEnviroment.SetVariable(id, value, context.Start);
        return defaultValue;
        
    }

    //VisitAssign 

    public override ValueWrapper VisitAssign(LanguageParser.AssignContext context)
    {
        string id = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        return currentEnviroment.AssignVariable(id, value, context.Start);
    }

    //VisitExprStmt
    public override ValueWrapper VisitExprStmt(LanguageParser.ExprStmtContext context)
    {
        return Visit(context.expr());
    }

    //VisitPrintStmt
    // public override ValueWrapper VisitPrintStmt(LanguageParser.PrintStmtContext context)
    // {
    //     ValueWrapper value = Visit(context.expr());
    //     output += value switch
    //     {
    //         IntValue i => i.Value.ToString(),
    //         FloatValue f => f.Value.ToString(),
    //         StringValue s => s.Value,
    //         BoolValue b => b.Value.ToString(),
    //         FuntionValue fn => "<fn "+fn.name+">",
    //         _ => throw new SemanticError("Tipo no reconocido", context.Start)
    //     };
    //     output += "\n";
    //     return defaultValue;
    // }


    //VisitParentheses
    public override ValueWrapper VisitParens(LanguageParser.ParensContext context)
    {
        return Visit(context.expr());
    }

    // VisitNegate

    public override ValueWrapper VisitNegate(LanguageParser.NegateContext context)
    {
        ValueWrapper value = Visit(context.expr());
        return value switch
        {
            IntValue i => new IntValue(-i.Value),
            FloatValue f => new FloatValue(-f.Value),
            _ => throw new SemanticError("Tipo invalido", context.Start)
        };
    }

    // VisitInt
    public override ValueWrapper VisitInt(LanguageParser.IntContext context)
    {
        return  new IntValue(int.Parse(context.INT().GetText()));
    }

    // VisitMulDiv
    public override ValueWrapper VisitMulDiv(LanguageParser.MulDivContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (IntValue l, IntValue r, "*") => new IntValue(l.Value * r.Value),
            (IntValue l, IntValue r, "/") => new IntValue(l.Value / r.Value),
            (IntValue l, FloatValue r, "*") => new FloatValue(l.Value * r.Value),
            (IntValue l, FloatValue r, "/") => new FloatValue(l.Value / r.Value),
            (FloatValue l, IntValue r, "*") => new FloatValue(l.Value * r.Value),
            (FloatValue l, IntValue r, "/") => new FloatValue(l.Value / r.Value),
            (FloatValue l, FloatValue r, "*") => new FloatValue(l.Value * r.Value),
            (FloatValue l, FloatValue r, "/") => new FloatValue(l.Value / r.Value),
            _ => throw new SemanticError("Operacion Invalida", context.Start)
        };
    }

    // VisitAddSub
    public override ValueWrapper VisitAddSub(LanguageParser.AddSubContext context)
    {
        ValueWrapper left = Visit(context.GetChild(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (IntValue l, IntValue r, "+") => new IntValue(l.Value + r.Value),
            (IntValue l, IntValue r, "-") => new IntValue(l.Value - r.Value),
            (IntValue l, FloatValue r, "+") => new FloatValue(l.Value + r.Value),
            (IntValue l, FloatValue r, "-") => new FloatValue(l.Value - r.Value),
            (FloatValue l, IntValue r, "+") => new FloatValue(l.Value + r.Value),
            (FloatValue l, IntValue r, "-") => new FloatValue(l.Value - r.Value),
            (FloatValue l, FloatValue r, "+") => new FloatValue(l.Value + r.Value),
            (FloatValue l, FloatValue r, "-") => new FloatValue(l.Value - r.Value),
            _ => throw new SemanticError("Operacion Invalida", context.Start)
        };
    }

    //VisitIdentifier
    public override ValueWrapper VisitIdentifier(LanguageParser.IdentifierContext context)
    {
        string id = context.ID().GetText();
        return currentEnviroment.GetVariable(id, context.Start);
    }

    //VisitFloat
    public override ValueWrapper VisitFloat(LanguageParser.FloatContext context)
    {
        return new FloatValue(float.Parse(context.FLOAT().GetText()));
    }

    //VisitBool
    public override ValueWrapper VisitBoolean(LanguageParser.BooleanContext context)
    {
        return new BoolValue(bool.Parse(context.BOOL().GetText()));
    }

    //VisitString
    public override ValueWrapper VisitString(LanguageParser.StringContext context)
    {
        return new StringValue(context.STRING().GetText());
    }

    //VisitRelational
    public override ValueWrapper VisitRelational(LanguageParser.RelationalContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (IntValue l, IntValue r, "<") => new BoolValue(l.Value < r.Value),
            (IntValue l, IntValue r, ">") => new BoolValue(l.Value > r.Value),
            (IntValue l, IntValue r, "<=") => new BoolValue(l.Value <= r.Value),
            (IntValue l, IntValue r, ">=") => new BoolValue(l.Value >= r.Value),
            (IntValue l, IntValue r, "==") => new BoolValue(l.Value == r.Value),
            (IntValue l, IntValue r, "!=") => new BoolValue(l.Value != r.Value),
            (FloatValue l, FloatValue r, "<") => new BoolValue(l.Value < r.Value),
            (FloatValue l, FloatValue r, ">") => new BoolValue(l.Value > r.Value),
            (FloatValue l, FloatValue r, "<=") => new BoolValue(l.Value <= r.Value),
            (FloatValue l, FloatValue r, ">=") => new BoolValue(l.Value >= r.Value),
            (FloatValue l, FloatValue r, "==") => new BoolValue(l.Value == r.Value),
            (FloatValue l, FloatValue r, "!=") => new BoolValue(l.Value != r.Value),
            _ => throw new SemanticError("Operacion Invalida", context.Start)
        };
    }

    //VisitEqualitys
    public override ValueWrapper VisitEqualitys(LanguageParser.EqualitysContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (IntValue l, IntValue r, "==") => new BoolValue(l.Value == r.Value),
            (IntValue l, IntValue r, "!=") => new BoolValue(l.Value != r.Value),
            (FloatValue l, FloatValue r, "==") => new BoolValue(l.Value == r.Value),
            (FloatValue l, FloatValue r, "!=") => new BoolValue(l.Value != r.Value),
            (BoolValue l, BoolValue r, "==") => new BoolValue(l.Value == r.Value),
            (BoolValue l, BoolValue r, "!=") => new BoolValue(l.Value != r.Value),
            (StringValue l, StringValue r, "==") => new BoolValue(l.Value == r.Value),
            (StringValue l, StringValue r, "!=") => new BoolValue(l.Value != r.Value),
            _ => throw new SemanticError("Operacion Invalida", context.Start)
        };
    }

    //VisitBlockStmt
    public override ValueWrapper VisitBlockStmt(LanguageParser.BlockStmtContext context)
    {
        Environment previousEnvironment = currentEnviroment;
        currentEnviroment =  new Environment(previousEnvironment);

        foreach (var stmt in context.dcl())
        {
            Visit(stmt);
        }
        currentEnviroment = previousEnvironment;
        return defaultValue;
    }

    //VisitIfStmt
    public override ValueWrapper VisitIfStmt(LanguageParser.IfStmtContext context)
    {
        ValueWrapper condition = Visit(context.expr());
        if (condition is not BoolValue)
        {
            throw new SemanticError("Condicion Invalida", context.Start);
        }
        if((condition as BoolValue).Value)
        {
            Visit(context.stmt(0));
        }
        else if(context.stmt().Length > 1)
        {
            Visit(context.stmt(1));
        }
        return defaultValue;

    }

    //VisitWhileStmt
    public override ValueWrapper VisitWhileStmt(LanguageParser.WhileStmtContext context)
    {
        ValueWrapper condition = Visit(context.expr());
        if (condition is not BoolValue)
        {
            throw new SemanticError("Condicion Invalida", context.Start);
        }
        while((condition as BoolValue).Value)
        {
            Visit(context.stmt());
            condition = Visit(context.expr());
            if (condition is not BoolValue)
            {
                throw new SemanticError("Condicion Invalida", context.Start);
            }
        }
        return defaultValue;
    }

    //VisitForStmt

    public override ValueWrapper VisitForStmt(LanguageParser.ForStmtContext context)
    {
        Environment previousEnvironment = currentEnviroment;
        currentEnviroment = new Environment(previousEnvironment);

        Visit(context.forInit());

        VisitForBody(context);

        currentEnviroment = previousEnvironment;
        return defaultValue;
    }

    public void VisitForBody(LanguageParser.ForStmtContext context)
    {
        ValueWrapper condition = Visit(context.expr(0));

        var lastEnvirontment = currentEnviroment;

        if (condition is not BoolValue)
        {
            throw new SemanticError("Condicion Invalida", context.Start);
        }
        try
        {
            while ((condition as BoolValue).Value)
            {
                Visit(context.stmt());
                Visit(context.expr(1));
                condition = Visit(context.expr(0));
            }
        }
        catch (BreakException)
        {
            currentEnviroment = lastEnvirontment;
        }
        catch (ContinueException)
        {
            currentEnviroment = lastEnvirontment;
            Visit(context.expr(1));
            VisitForBody(context);
        }
    }


    //VisitBreakStmt
    public override ValueWrapper VisitBreakStmt(LanguageParser.BreakStmtContext context)
    {
        throw new BreakException();
    }
    //VisitContinueStmt
    public override ValueWrapper VisitContinueStmt(LanguageParser.ContinueStmtContext context)
    {
        throw new ContinueException();
    }
    //VisitReturnStmt
    public override ValueWrapper VisitReturnStmt(LanguageParser.ReturnStmtContext context)
    {
        ValueWrapper? value = defaultValue;

        if (context.expr() != null)
        {
            value = Visit(context.expr());
        }

        throw new ReturnException(value);
    }


    //VisitCallee
    public override ValueWrapper VisitCallee(LanguageParser.CalleeContext context)
    {
        ValueWrapper callee = Visit(context.expr());

        foreach(var call in context.call())
        {
            if(callee is FuntionValue funtionValue){
                callee = VisitCall(funtionValue.invocable, call.args());
            }
            else{
                throw new SemanticError("No es una funcion", context.Start);
            }
        }
        return callee;
    }

    public ValueWrapper VisitCall(Invocable invocable, LanguageParser.ArgsContext context)
    {
        List<ValueWrapper> arguments = new List<ValueWrapper>();
        if(context != null)
        {
            foreach(var expr in context.expr())
            {
                arguments.Add(Visit(expr));
            }
        }

        // if(context != null && arguments.Count != invocable.Arity())
        // {
        //     throw new SemanticError("Numero de argumentos incorrecto", context.Start);
        // }

        return invocable.Invoke(arguments, this);
    }

}