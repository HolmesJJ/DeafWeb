function [dist,diff] = dtw(t,r)
n = size(t,1);
m = size(r,1);

% Ö¡Æ¥Åä¾àÀë¾ØÕó
d = zeros(n,m);

for i = 1:n
for j = 1:m
	d(i,j) = sum((t(i,:)-r(j,:)).^2);
end
end

% ÀÛ»ý¾àÀë¾ØÕó
D =  ones(n,m) * realmax;
D(1,1) = d(1,1);
D_index = ones(n,m);

% ¶¯Ì¬¹æ»®
for i = 2:n
for j = 1:m
	D1 = D(i-1,j);

	if j>1
		D2 = D(i-1,j-1);
    else
        D2 = realmax;
    end
	if j>1
		D3 = D(i,j-1);
    else
        D3 = realmax;
    end
    dmin=   min([D1,D2,D3]);
    dmin_index = find([D1,D2,D3]==dmin);
	D(i,j) = d(i,j) + dmin;
    D_index (i,j) = dmin_index(1); 
end
end
diff =[];
i = n;
j = m;
while i > 1 && j > 1
    diff = [diff d(i,j)];
    d_index = D_index(i,j);
    if d_index == 1
        i = i-1;
    end
    if d_index == 2
        i = i-1;
        j = j-1;
    end
   if d_index == 3
        j = j-1;
   end
end

diff =sqrt(diff);
diff = fliplr(diff);
dist = D(n,m);