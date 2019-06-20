# KMeansIris
KMeans implementation with Iris as the dataset

## KMeans Configuration
- Clusters count - this is the easiest one. Since we know the nature of the data, the obvious chouse is 3.
- Maximum iterations - 100.
- KMeans clusterizations - 25, with different initial clusters.

## Used Distance functions
- **EuclideanDistance** - the most simple and straight forward distance function;
- **ManhattanDistance** - just for curiosity, I expect the worst results from this function;
- **CosineDistance** - this function is used when the magnitude between vectors does not matter but the orientation does.
- **ChebyshevDistance** - in there, just because I am a chess fan :)
- And others.

## Best Results
| Pos | Distance | Errors | Errors % | Ratio |
|--|--|--|--|--|
| 1 | Cosine | 5 | 3.33% | 145/150 | 
| 2 | Chebyshev | 10 | 6.67% | 140/150 | 
| 3 | Euclidean | 16 | 10.67% | 134/150 | 
| 4 | Manhattan | 17 | 11.33% | 133/150 | 

## Oher Distance functions
Many other distance functions were used, all of them had results worse than Manhattan distance. Some of them (ex. Hamming Distance) even put all of the items in the same cluster.