% ����������ͬ����������Ƶ��
[d1,sr] = audioread('au.mp3');
[d2,sr] = audioread('au.mp3');
d1 = d1(:,1);

d2 = d2(:,1);

% һ������������Ƶ
ml = min(length(d1),length(d2));
%soundsc(d1(1:ml)+d2(1:ml),sr)
% Ҳ���԰��������������
%soundsc([d1(1:ml),d2(1:ml)],sr)

%�������������ж�ʱ����Ҷ�任STFT��20%�Ĵ����۵���
D1 = specgram(d1,512,sr,512,384);
D2 = specgram(d2,512,sr,512,384);
%����Requires vector (either row or column) input.����Ϊ����ص�����������

%����STFT����ƥ�䷽�󣬲�����������ģ������������ÿ��Ƭ�ε���Ծ��롣
SM = simmx(abs(D1),abs(D2));

%��ʾ
 figure(1);
subplot(121)
imagesc(SM)
colormap(1-gray)
% ;�п��Կ���һ���ڰ������ƣ���������ֵ������һ�����µĶԽ��ߡ��ö�̬�滮��������·����·�����Ӿ�������ǡ�
[p,q,C] = dp(1-SM);
hold on; plot(q,p,'r'); hold off

subplot(122)
imagesc(C)
hold on; plot(q,p,'r'); hold off

C(size(C,1),size(C,2))

%���ֵΪ·�����ģ��������Ϊ���ƶȣ�ʹ�ò�ͬģ�����ƥ�䣬��Сֵ˵��ƥ������·����

 % ��������Ϣ���ʱ��
 D2i1 = zeros(1, size(D1,2));
 for i = 1:length(D2i1)
     D2i1(i) = q(min(find(p >= i))); 
 end
 D2x = pvsample(D2, D2i1-1, 128);
 
 %ת��Ϊʱ��
 d2x = istft(D2x, 512, 512, 128);
 d2x = resize(d2x', length(d1),1);
 figure(2);
 subplot(211);
 plot(d1)
 subplot(212);
 plot(d2x)
 figure(3);
  subplot(211);
 y1=fft(d1);       %��FFT�任
f=(0:length(y1)-1).*sr/length(y1);
plot(f,y1)       %����ԭʼ�źŵ�Ƶ��ͼ
 subplot(212);
  y2=fft(d2x);       %��FFT�任
f=(0:length(y2)-1).*sr/length(y2);
plot(f,y2)       %����ԭʼ�źŵ�Ƶ��ͼ
 %��һ�½����
 % �����任������
 %soundsc(d2x,sr)

 %soundsc(d1+d2x,sr)
 % ������
 %soundsc([d1,d2x],sr)
 % ��û�任���������жԱ�
 %soundsc([d1(1:ml),d2(1:ml)],sr)
