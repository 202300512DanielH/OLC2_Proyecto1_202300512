using analyzer;

public class ForeignFuntion : Invocable{

    private Environment closure;
    private LanguageParser.FuncDclContext context;

    public ForeignFuntion(Environment closure, LanguageParser.FuncDclContext context){
        this.closure = closure;
        this.context = context;
    }

    public int Arity(){
        if (context.@params() == null){
            return 0;
        }
        return context.@params().ID().Length;
    }

    public ValueWrapper Invoke(List<ValueWrapper> args, CompilerVisitor visitor){
        var newEnv = new Environment(closure);
        var beforeCallEnv = visitor.currentEnviroment;
        visitor.currentEnviroment = newEnv;

        if (context.@params() != null){
            for (int i = 0; i < context.@params().ID().Length; i++){
                newEnv.SetVariable(context.@params().ID(i).GetText(), args[i], null);
            }
        }

        try{
            foreach (var stmt in context.dcl()){
            visitor.Visit(stmt);
        }
        }
        catch (ReturnException e){
            visitor.currentEnviroment = beforeCallEnv;
            return e.Value;
        }

        visitor.currentEnviroment = beforeCallEnv;
        return visitor.defaultValue;
    }


}