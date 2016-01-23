namespace RDFCommon
{
    public interface ILiteralNode
    {  
        string DataType { get; }
        dynamic Content { get; }
    }
}