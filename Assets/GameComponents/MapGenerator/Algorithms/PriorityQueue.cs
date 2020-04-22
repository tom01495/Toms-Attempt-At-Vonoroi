public class PriorityQueue<T> where T : System.IComparable<T> {
    private readonly T[] heap;
    private uint count;

    // TODO Implement this!

    public PriorityQueue(uint max_size){
        heap = new T[(uint)max_size + 1];
        count = 0;
    }

    public bool Empty() => count == 0;

    public void Enqueue(T s){
        uint index = ++count;

        uint? parent = Parent(index);
        while(parent.HasValue && s.CompareTo(heap[parent.Value]) == -1){
            heap[index] = heap[parent.Value];
            index = parent.Value;
            parent = Parent(index);
        }

        heap[index] = s;
    }

    private uint? Parent(uint i){
        if(i == 1) { return null; }
        return i/2;
    }
    private uint? Left(uint i){
        if(2*i <= count) { return 2*i; }
        return null;
    }
    private uint? Right(uint i){
        if(2*i + 1 <= count) { return 2*i+1; }
        return null;
    }

    public T Dequeue(){
        T e = heap[0];
        uint index;
        uint smallest = 1;

        do {
            index = smallest;
            uint? left = Left(index);
            uint? right = Right(index);

            if(left.HasValue && heap[left.Value].CompareTo(heap[smallest]) == -1)
                smallest = left.Value;

            if(right.HasValue && heap[right.Value].CompareTo(heap[smallest]) == -1)
                smallest = right.Value;

            if (smallest != index){
                T temp = heap[index];
                heap[index] = heap[smallest];
                heap[smallest] = temp;
            }
        } while(index != smallest);

        count--;
        return e;
    }
}