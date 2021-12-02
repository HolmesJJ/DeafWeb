function [p,q,D,sc] = dpfast(M,C,T,G)
% ���᷵�غ�dp.mһ���Ľ�������������200�౶��
% C������һ���߶���Ĭ��[1 1 1.0;0 1 1.0;1 0 1.0]�����ǲ���[1 1 1;1 0 1;0 1 1;1 2 2;2 1 2]����
% T��׷����㣺0������ߣ�1�����Ϸ���Ĭ�ϣ���T>1�ǻ�ó���С���Խ��ߡ�
% G���� T=0ʱ��G �����ˡ�gulleys���ĳ��ȣ��������dpwe@ee.columbia.edu�޸�


if nargin < 2
  % Default step / cost matrix
  C = [1 1 1.0;0 1 1.0;1 0 1.0];
end

if nargin < 3
  % Default: path to top-right
  T = 1;
end

if nargin < 4
  % how big are gulleys?
  G = 0.5;  % half the extent
end

if sum(isnan(M(:)))>0
  error('dpwe:dpfast:NAN','Error: Cost matrix includes NaNs');
end

if min(M(:)) < 0
  disp('Warning: cost matrix includes negative values; results may not be what you expect');
end

[r,c] = size(M);

% Core cumulative cost calculation coded as mex
[D,phi] = dpcore(M,C);

p = [];
q = [];

% Traceback from top left?
%i = r; 
%j = c;

if T == 0
  % Traceback from lowest cost "to edge" (gulleys)
  TE = D(r,:);
  RE = D(:,c);
  % eliminate points not in gulleys
  TE(1:round((1-G)*c)) = max(max(D));
  RE(1:round((1-G)*r)) = max(max(D));
  if (min(TE) < min(RE))
    i = r;
    j = max(find(TE==min(TE)));
  else
    i = max(find(RE==min(RE)));
    j = c;
  end
else
  if min(size(D)) == 1
    % degenerate D has only one row or one column - messes up diag
    i = r;
    j = c;
  else
    % Traceback from min of antidiagonal
    %stepback = floor(0.1*c);
    stepback = T;
    slice = diag(fliplr(D),-(r-stepback));
    [mm,ii] = min(slice);
    i = r - stepback + ii;
    j = c + 1 - ii;
  end
end

p=i;
q=j;

sc = M(p,q);

while i > 1 && j > 1
%  disp(['i=',num2str(i),' j=',num2str(j)]);
  tb = phi(i,j);
  i = i - C(tb,1);
  j = j - C(tb,2);
  p = [i,p];
  q = [j,q];
  sc = [M(i,j),sc];
end