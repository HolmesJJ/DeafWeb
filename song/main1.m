% 加载两个相同读音的声音频段
[d1,sr] = audioread('au.mp3');
[d2,sr] = audioread('au.mp3');
d1 = d1(:,1);

d2 = d2(:,1);

% 一起听这两段音频
ml = min(length(d1),length(d2));
%soundsc(d1(1:ml)+d2(1:ml),sr)
% 也可以把他们组成立体声
%soundsc([d1(1:ml),d2(1:ml)],sr)

%对两个声音进行短时傅里叶变换STFT（20%的窗口折叠）
D1 = specgram(d1,512,sr,512,384);
D2 = specgram(d2,512,sr,512,384);
%遇到Requires vector (either row or column) input.是因为你加载的是立体声。

%基于STFT构建匹配方阵，并计算样本和模板声音样本的每个片段的相对距离。
SM = simmx(abs(D1),abs(D2));

%显示
 figure(1);
subplot(121)
imagesc(SM)
colormap(1-gray)
% 途中可以看到一个黑暗的条纹（高相似性值），是一个向下的对角线。用动态规划法求最优路径，路径连接矩阵的两角。
[p,q,C] = dp(1-SM);
hold on; plot(q,p,'r'); hold off

subplot(122)
imagesc(C)
hold on; plot(q,p,'r'); hold off

C(size(C,1),size(C,2))

%这个值为路径消耗，可以理解为相似度，使用不同模板进行匹配，最小值说明匹配最优路径。

 % 把声音信息变回时域
 D2i1 = zeros(1, size(D1,2));
 for i = 1:length(D2i1)
     D2i1(i) = q(min(find(p >= i))); 
 end
 D2x = pvsample(D2, D2i1-1, 128);
 
 %转化为时域
 d2x = istft(D2x, 512, 512, 128);
 d2x = resize(d2x', length(d1),1);
 figure(2);
 subplot(211);
 plot(d1)
 subplot(212);
 plot(d2x)
 figure(3);
  subplot(211);
 y1=fft(d1);       %做FFT变换
f=(0:length(y1)-1).*sr/length(y1);
plot(f,y1)       %画出原始信号的频谱图
 subplot(212);
  y2=fft(d2x);       %做FFT变换
f=(0:length(y2)-1).*sr/length(y2);
plot(f,y2)       %画出原始信号的频谱图
 %听一下结果吧
 % 经过变换的声音
 %soundsc(d2x,sr)

 %soundsc(d1+d2x,sr)
 % 立体声
 %soundsc([d1,d2x],sr)
 % 和没变换的声音进行对比
 %soundsc([d1(1:ml),d2(1:ml)],sr)
