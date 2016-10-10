# [WPF] Custom header for DataGrid
This UserControl allows create header for DataGrid with spaned cells. <br><br>

All you need is pass input "Pattern", in which you need to set: <br>
1) "Base Grid" parameters (like width and height of cells), <br>
2) "Template" for each custom Cell (contains: row and column indexes, row and column spans, text content of cell) <br>
3) "Binding" for each column, to associate data with columns <br>
4) "DataSource" which you wont to be shown in DataGrid <br>
5) And as bonus you can set the number of frozen columns starting from left side.
