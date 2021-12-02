function M = simmx(A,B)

EA = sqrt(sum(A.^2));
EB = sqrt(sum(B.^2));

M = (A'*B)./(EA'*EB);