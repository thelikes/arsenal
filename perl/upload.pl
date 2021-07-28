#!/bin/perl
use LWP::Simple;

if ($#ARGV == 1)
{
	print "saving " . $ARGV[0] . " as " . $ARGV[1] . "\n";
	print getstore($ARGV[0], $ARGV[1]);
}