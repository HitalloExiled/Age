typealias Vector2<T> = vector<T, 2>;

struct Transform<T>
{
    Vector2<T> rotation;
    Vector2<T> position;
};
