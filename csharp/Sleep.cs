static extern void Sleep(uint dwMilliseconds);
// main
DateTime t1 = DateTime.Now;
Sleep(2000);
double t2 = DateTime.Now.Subtract(t1).TotalSeconds;
if (t2 < 1.5)
{
	return;
}
