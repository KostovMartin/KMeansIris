# KMeansIris | AgnesIris
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
| Pos | Algorithm |  Distance | Errors | Errors % | Correct | Total |
|--|--|--|--|--|--|--|
| 1 | KMeans | Cosine | 5 | 3.33% | 145 | 150 | 
| 2 | Agnes | Cosine + AverageWPGMA | 6 | 4.00% | 144 | 150 | 
| 3 | KMeans | Chebyshev | 10 | 6.67% | 140 | 150 | 
| 4 | Agnes | Euclidean + AverageUPGMA | 14 | 9.33% | 136 | 150 | 
| 5 | Agnes | Euclidean + AverageWPGMA | 15 | 10.00% | 135 | 150 | 
| 6 | KMeans | Euclidean | 16 | 10.67% | 134 | 150 | 
| 7 | KMeans | Manhattan | 17 | 11.33% | 133 | 150 | 
| 8 | Agnes | Cosine + CompleteLinkage | 24 | 16.00% | 126 | 150 |
| 9 | Agnes | Euclidean + CompleteLinkage | 24 | 16.00% | 126 | 150 | 
| 10 | Agnes | Euclidean + SingleLinkage | 48 | 32.00% | 102 | 150 |
| 11 | Agnes | Cosine + SingleLinkage | 50 | 33.33% | 100 | 150 
| 12 | Agnes | Cosine + AverageUPGMA | 50 | 33.33% | 100 | 150   

## Oher Distance functions
Many other distance functions were used, all of them had results worse than Manhattan distance. Some of them (ex. Hamming Distance) even put all of the items in the same cluster.